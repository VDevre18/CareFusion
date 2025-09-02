using AutoMapper;
using CareFusion.Business.Services;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using FluentValidation;
using Moq;
using Xunit;

namespace CareFusion.Business.Tests;

public class PatientManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<PatientDto>> _mockValidator;
    private readonly PatientManager _patientManager;

    public PatientManagerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<PatientDto>>();
        
        // Setup mock repositories
        var mockPatientRepository = new Mock<CareFusion.Core.Repositories.Interfaces.IPatientRepository>();
        _mockUow.Setup(x => x.Patients).Returns(mockPatientRepository.Object);
        
        _patientManager = new PatientManager(_mockUow.Object, _mockMapper.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPatient()
    {
        // Arrange
        var patientId = 1;
        var patient = new Patient 
        { 
            Id = patientId, 
            FirstName = "John", 
            LastName = "Doe",
            MRN = "TEST001"
        };
        var patientDto = new PatientDto 
        { 
            Id = patientId, 
            FirstName = "John", 
            LastName = "Doe",
            MRN = "TEST001"
        };

        _mockUow.Setup(x => x.Patients.GetWithExamsAsync(patientId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(patient);
        _mockMapper.Setup(x => x.Map<PatientDto>(patient)).Returns(patientDto);

        // Act
        var result = await _patientManager.GetByIdAsync(patientId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(patientId, result.Data.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsFailure()
    {
        // Arrange
        var patientId = 2;
        _mockUow.Setup(x => x.Patients.GetWithExamsAsync(patientId, It.IsAny<CancellationToken>()))
               .ReturnsAsync((Patient?)null);

        // Act
        var result = await _patientManager.GetByIdAsync(patientId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Patient not found", result.Message);
    }

    [Fact]
    public async Task CreateAsync_WithValidPatient_ReturnsSuccess()
    {
        // Arrange
        var patientDto = new PatientDto
        {
            Id = 3,
            FirstName = "Jane",
            LastName = "Smith",
            MRN = "TEST002"
        };

        var patient = new Patient
        {
            Id = patientDto.Id,
            FirstName = "Jane",
            LastName = "Smith",
            MRN = "TEST002"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        
        _mockValidator.Setup(x => x.ValidateAsync(patientDto, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);
        _mockUow.Setup(x => x.Patients.ExistsByMrnAsync("TEST002", It.IsAny<CancellationToken>()))
               .ReturnsAsync(false);
        _mockMapper.Setup(x => x.Map<Patient>(patientDto)).Returns(patient);
        _mockMapper.Setup(x => x.Map<PatientDto>(patient)).Returns(patientDto);

        // Act
        var result = await _patientManager.CreateAsync(patientDto, "test-user");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Patient created successfully", result.Message);
        _mockUow.Verify(x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(x => x.SaveChangesAsync("test-user", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateMRN_ReturnsFailure()
    {
        // Arrange
        var patientDto = new PatientDto
        {
            Id = 4,
            FirstName = "Jane",
            LastName = "Smith",
            MRN = "TEST002"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        
        _mockValidator.Setup(x => x.ValidateAsync(patientDto, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);
        _mockUow.Setup(x => x.Patients.ExistsByMrnAsync("TEST002", It.IsAny<CancellationToken>()))
               .ReturnsAsync(true);

        // Act
        var result = await _patientManager.CreateAsync(patientDto, "test-user");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Patient with MRN TEST002 already exists", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidPatient_ReturnsSuccess()
    {
        // Arrange
        var patientId = 5;
        var patientDto = new PatientDto
        {
            Id = patientId,
            FirstName = "John Updated",
            LastName = "Doe",
            MRN = "TEST001"
        };

        var existingPatient = new Patient
        {
            Id = patientId,
            FirstName = "John",
            LastName = "Doe",
            MRN = "TEST001"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        
        _mockValidator.Setup(x => x.ValidateAsync(patientDto, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);
        _mockUow.Setup(x => x.Patients.GetByIdAsync(patientId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingPatient);
        _mockMapper.Setup(x => x.Map<PatientDto>(existingPatient)).Returns(patientDto);

        // Act
        var result = await _patientManager.UpdateAsync(patientDto, "test-user");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Patient updated successfully", result.Message);
        _mockMapper.Verify(x => x.Map(patientDto, existingPatient), Times.Once);
        _mockUow.Verify(x => x.Patients.Update(existingPatient), Times.Once);
        _mockUow.Verify(x => x.SaveChangesAsync("test-user", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class ExamManagerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<ExamDto>> _mockValidator;
    private readonly ExamManager _examManager;

    public ExamManagerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<ExamDto>>();
        
        // Setup mock repositories
        var mockExamRepository = new Mock<CareFusion.Core.Repositories.Interfaces.IExamRepository>();
        _mockUow.Setup(x => x.Exams).Returns(mockExamRepository.Object);
        
        _examManager = new ExamManager(_mockUow.Object, _mockMapper.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsExam()
    {
        // Arrange
        var examId = 10;
        var exam = new Exam
        {
            Id = examId,
            PatientId = 11,
            Modality = "CT",
            StudyType = "Chest CT",
            StudyDateUtc = DateTime.UtcNow,
            Status = "New"
        };
        var examDto = new ExamDto
        {
            Id = examId,
            PatientId = exam.PatientId,
            Modality = "CT",
            StudyType = "Chest CT",
            StudyDateUtc = exam.StudyDateUtc,
            Status = "New"
        };

        _mockUow.Setup(x => x.Exams.GetByIdAsync(examId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(exam);
        _mockMapper.Setup(x => x.Map<ExamDto>(exam)).Returns(examDto);

        // Act
        var result = await _examManager.GetByIdAsync(examId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(examId, result.Data.Id);
    }

    [Fact]
    public async Task ListByPatientAsync_WithValidPatientId_ReturnsExams()
    {
        // Arrange
        var patientId = 12;
        var exams = new List<Exam>
        {
            new() { Id = 13, PatientId = patientId, Modality = "CT", StudyType = "Chest CT", StudyDateUtc = DateTime.UtcNow },
            new() { Id = 14, PatientId = patientId, Modality = "MRI", StudyType = "Brain MRI", StudyDateUtc = DateTime.UtcNow.AddDays(-1) }
        };

        var examDtos = exams.Select(e => new ExamDto
        {
            Id = e.Id,
            PatientId = e.PatientId,
            Modality = e.Modality,
            StudyType = e.StudyType,
            StudyDateUtc = e.StudyDateUtc
        }).ToList();

        _mockUow.Setup(x => x.Exams.ListByPatientAsync(patientId, 0, 10, It.IsAny<CancellationToken>()))
               .ReturnsAsync((exams, exams.Count));
        
        foreach (var (exam, dto) in exams.Zip(examDtos))
        {
            _mockMapper.Setup(x => x.Map<ExamDto>(exam)).Returns(dto);
        }

        // Act
        var result = await _examManager.ListByPatientAsync(patientId, 1, 10);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count);
        Assert.Equal(2, result.Data.TotalCount);
    }

    [Fact]
    public async Task CreateAsync_WithValidExam_ReturnsSuccess()
    {
        // Arrange
        var examDto = new ExamDto
        {
            Id = 15,
            PatientId = 16,
            Modality = "X-Ray",
            StudyType = "Chest X-Ray",
            StudyDateUtc = DateTime.UtcNow,
            Status = "New"
        };

        var exam = new Exam
        {
            Id = examDto.Id,
            PatientId = examDto.PatientId,
            Modality = examDto.Modality,
            StudyType = examDto.StudyType,
            StudyDateUtc = examDto.StudyDateUtc,
            Status = examDto.Status
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        
        _mockValidator.Setup(x => x.ValidateAsync(examDto, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);
        _mockMapper.Setup(x => x.Map<Exam>(examDto)).Returns(exam);
        _mockMapper.Setup(x => x.Map<ExamDto>(exam)).Returns(examDto);

        // Act
        var result = await _examManager.CreateAsync(examDto, "test-user");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Exam created successfully", result.Message);
        _mockUow.Verify(x => x.Exams.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(x => x.SaveChangesAsync("test-user", It.IsAny<CancellationToken>()), Times.Once);
    }
}
