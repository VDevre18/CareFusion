using CareFusion.Core.Repositories;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CareFusion.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CareFusionDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
