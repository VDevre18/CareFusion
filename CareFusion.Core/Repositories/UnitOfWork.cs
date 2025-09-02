using CareFusion.Core.Repositories.Interfaces;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Unit of Work implementation that provides access to all repositories and manages database transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CareFusionDbContext _db;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork with all required repositories
    /// </summary>
    /// <param name="db">Database context for transaction management</param>
    /// <param name="patients">Repository for patient data operations</param>
    /// <param name="exams">Repository for exam data operations</param>
    /// <param name="users">Repository for user data operations</param>
    /// <param name="patientNotes">Repository for patient note data operations</param>
    /// <param name="patientReports">Repository for patient report data operations</param>
    /// <param name="examImages">Repository for exam image data operations</param>
    /// <param name="clinicSites">Repository for clinic site data operations</param>
    public UnitOfWork(CareFusionDbContext db,
                      IPatientRepository patients,
                      IExamRepository exams,
                      IUserRepository users,
                      IPatientNoteRepository patientNotes,
                      IPatientReportRepository patientReports,
                      IExamImageRepository examImages,
                      IClinicSiteRepository clinicSites)
    {
        _db = db;
        Patients = patients;
        Exams = exams;
        Users = users;
        PatientNotes = patientNotes;
        PatientReports = patientReports;
        ExamImages = examImages;
        ClinicSites = clinicSites;
    }

    /// <summary>
    /// Repository for managing patients
    /// </summary>
    public IPatientRepository Patients { get; }
    
    /// <summary>
    /// Repository for managing medical exams
    /// </summary>
    public IExamRepository Exams { get; }
    
    /// <summary>
    /// Repository for managing system users
    /// </summary>
    public IUserRepository Users { get; }
    
    /// <summary>
    /// Repository for managing patient notes
    /// </summary>
    public IPatientNoteRepository PatientNotes { get; }
    
    /// <summary>
    /// Repository for managing patient reports
    /// </summary>
    public IPatientReportRepository PatientReports { get; }
    
    /// <summary>
    /// Repository for managing exam images
    /// </summary>
    public IExamImageRepository ExamImages { get; }
    
    /// <summary>
    /// Repository for managing clinic sites
    /// </summary>
    public IClinicSiteRepository ClinicSites { get; }

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="user">The username of the person making the changes</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of entities affected</returns>
    public Task<int> SaveChangesAsync(string? user = null, CancellationToken ct = default)
        => _db.SaveChangesAsync(user, ct);

    /// <summary>
    /// Disposes the database context asynchronously
    /// </summary>
    /// <returns>ValueTask for async disposal</returns>
    public ValueTask DisposeAsync() => _db.DisposeAsync();
}
