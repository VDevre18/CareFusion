namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Unit of Work interface that provides access to all repositories and manages database transactions
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Repository for managing patients
    /// </summary>
    IPatientRepository Patients { get; }
    
    /// <summary>
    /// Repository for managing medical exams
    /// </summary>
    IExamRepository Exams { get; }
    
    /// <summary>
    /// Repository for managing system users
    /// </summary>
    IUserRepository Users { get; }
    
    /// <summary>
    /// Repository for managing patient notes
    /// </summary>
    IPatientNoteRepository PatientNotes { get; }
    
    /// <summary>
    /// Repository for managing patient reports
    /// </summary>
    IPatientReportRepository PatientReports { get; }
    
    /// <summary>
    /// Repository for managing exam images
    /// </summary>
    IExamImageRepository ExamImages { get; }
    
    /// <summary>
    /// Repository for managing clinic sites
    /// </summary>
    IClinicSiteRepository ClinicSites { get; }

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="user">The username of the person making the changes</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of entities affected</returns>
    Task<int> SaveChangesAsync(string? user = null, CancellationToken ct = default);
}
