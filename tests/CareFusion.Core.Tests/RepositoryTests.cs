using CareFusion.Core;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareFusion.Core.Tests;

public class PatientRepositoryTests : IDisposable
{
    private readonly CareFusionDbContext _context;
    private readonly PatientRepository _repository;

    public PatientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CareFusionDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CareFusionDbContext(options);
        _repository = new PatientRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPatientToDatabase()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            MRN = "TEST001",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = "Male"
        };

        // Act
        await _repository.AddAsync(patient);
        await _context.SaveChangesAsync();

        // Assert
        var savedPatient = await _context.Patients.FindAsync(patient.Id);
        Assert.NotNull(savedPatient);
        Assert.Equal("John", savedPatient.FirstName);
        Assert.Equal("Doe", savedPatient.LastName);
        Assert.Equal("TEST001", savedPatient.MRN);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPatient()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            MRN = "TEST002"
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(patient.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(invalidId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByMrnAsync_WithExistingMRN_ShouldReturnTrue()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Patient",
            MRN = "UNIQUE001"
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsByMrnAsync("UNIQUE001");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByMrnAsync_WithNonExistingMRN_ShouldReturnFalse()
    {
        // Act
        var exists = await _repository.ExistsByMrnAsync("NONEXISTENT");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnFilteredResults()
    {
        // Arrange
        var patients = new[]
        {
            new Patient { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", MRN = "TEST001" },
            new Patient { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", MRN = "TEST002" },
            new Patient { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Johnson", MRN = "TEST003" }
        };

        _context.Patients.AddRange(patients);
        await _context.SaveChangesAsync();

        // Act
        var (results, total) = await _repository.SearchAsync("John", 0, 10);

        // Assert
        Assert.Equal(2, total); // John Doe and Bob Johnson
        Assert.Equal(2, results.Count);
        Assert.Contains(results, p => p.FirstName == "John");
        Assert.Contains(results, p => p.LastName == "Johnson");
    }

    [Fact]
    public async Task SearchAsync_WithMRNTerm_ShouldReturnMatchingPatient()
    {
        // Arrange
        var patient = new Patient 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "Test", 
            LastName = "Patient", 
            MRN = "SEARCH001" 
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var (results, total) = await _repository.SearchAsync("SEARCH001", 0, 10);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(results);
        Assert.Equal("SEARCH001", results[0].MRN);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPatients()
    {
        // Arrange
        var patients = new[]
        {
            new Patient { Id = Guid.NewGuid(), FirstName = "Patient", LastName = "One", MRN = "ALL001" },
            new Patient { Id = Guid.NewGuid(), FirstName = "Patient", LastName = "Two", MRN = "ALL002" },
            new Patient { Id = Guid.NewGuid(), FirstName = "Patient", LastName = "Three", MRN = "ALL003" }
        };

        _context.Patients.AddRange(patients);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void Update_ShouldMarkPatientAsModified()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Original",
            LastName = "Name",
            MRN = "UPDATE001"
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();

        // Act
        patient.FirstName = "Updated";
        _repository.Update(patient);

        // Assert
        var entry = _context.Entry(patient);
        Assert.Equal(EntityState.Modified, entry.State);
    }

    [Fact]
    public void Remove_ShouldMarkPatientForDeletion()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Delete",
            LastName = "Me",
            MRN = "DELETE001"
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();

        // Act
        _repository.Remove(patient);

        // Assert
        var entry = _context.Entry(patient);
        Assert.Equal(EntityState.Deleted, entry.State);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class ExamRepositoryTests : IDisposable
{
    private readonly CareFusionDbContext _context;
    private readonly ExamRepository _repository;
    private readonly Patient _testPatient;

    public ExamRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CareFusionDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CareFusionDbContext(options);
        _repository = new ExamRepository(_context);

        // Create a test patient for exam relationships
        _testPatient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Patient",
            MRN = "EXAM_TEST_001"
        };
        _context.Patients.Add(_testPatient);
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAsync_ShouldAddExamToDatabase()
    {
        // Arrange
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = _testPatient.Id,
            Modality = "CT",
            StudyType = "Chest CT",
            StudyDateUtc = DateTime.UtcNow,
            Status = "New"
        };

        // Act
        await _repository.AddAsync(exam);
        await _context.SaveChangesAsync();

        // Assert
        var savedExam = await _context.Exams.FindAsync(exam.Id);
        Assert.NotNull(savedExam);
        Assert.Equal("CT", savedExam.Modality);
        Assert.Equal("Chest CT", savedExam.StudyType);
        Assert.Equal(_testPatient.Id, savedExam.PatientId);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnExam()
    {
        // Arrange
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = _testPatient.Id,
            Modality = "MRI",
            StudyType = "Brain MRI",
            StudyDateUtc = DateTime.UtcNow,
            Status = "In Progress"
        };
        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(exam.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(exam.Id, result.Id);
        Assert.Equal("MRI", result.Modality);
    }

    [Fact]
    public async Task ListByPatientAsync_ShouldReturnExamsForPatient()
    {
        // Arrange
        var exams = new[]
        {
            new Exam { Id = Guid.NewGuid(), PatientId = _testPatient.Id, Modality = "CT", StudyType = "Chest CT", StudyDateUtc = DateTime.UtcNow.AddDays(-2) },
            new Exam { Id = Guid.NewGuid(), PatientId = _testPatient.Id, Modality = "MRI", StudyType = "Brain MRI", StudyDateUtc = DateTime.UtcNow.AddDays(-1) },
            new Exam { Id = Guid.NewGuid(), PatientId = Guid.NewGuid(), Modality = "X-Ray", StudyType = "Chest X-Ray", StudyDateUtc = DateTime.UtcNow } // Different patient
        };

        _context.Exams.AddRange(exams);
        await _context.SaveChangesAsync();

        // Act
        var (results, total) = await _repository.ListByPatientAsync(_testPatient.Id, 0, 10);

        // Assert
        Assert.Equal(2, total);
        Assert.Equal(2, results.Count);
        Assert.All(results, exam => Assert.Equal(_testPatient.Id, exam.PatientId));
        
        // Should be ordered by StudyDateUtc descending
        Assert.True(results[0].StudyDateUtc > results[1].StudyDateUtc);
    }

    [Fact]
    public async Task ListByPatientAsync_WithPaging_ShouldReturnPagedResults()
    {
        // Arrange
        var exams = Enumerable.Range(1, 5).Select(i => new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = _testPatient.Id,
            Modality = "CT",
            StudyType = $"Study {i}",
            StudyDateUtc = DateTime.UtcNow.AddDays(-i),
            Status = "New"
        }).ToArray();

        _context.Exams.AddRange(exams);
        await _context.SaveChangesAsync();

        // Act
        var (results, total) = await _repository.ListByPatientAsync(_testPatient.Id, 1, 2); // Skip 1, take 2

        // Assert
        Assert.Equal(5, total);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Update_ShouldMarkExamAsModified()
    {
        // Arrange
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = _testPatient.Id,
            Modality = "X-Ray",
            StudyType = "Original Study",
            StudyDateUtc = DateTime.UtcNow,
            Status = "New"
        };
        _context.Exams.Add(exam);
        _context.SaveChanges();

        // Act
        exam.StudyType = "Updated Study";
        _repository.Update(exam);

        // Assert
        var entry = _context.Entry(exam);
        Assert.Equal(EntityState.Modified, entry.State);
    }

    [Fact]
    public void Remove_ShouldMarkExamForDeletion()
    {
        // Arrange
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = _testPatient.Id,
            Modality = "Ultrasound",
            StudyType = "Delete Me",
            StudyDateUtc = DateTime.UtcNow,
            Status = "New"
        };
        _context.Exams.Add(exam);
        _context.SaveChanges();

        // Act
        _repository.Remove(exam);

        // Assert
        var entry = _context.Entry(exam);
        Assert.Equal(EntityState.Deleted, entry.State);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
