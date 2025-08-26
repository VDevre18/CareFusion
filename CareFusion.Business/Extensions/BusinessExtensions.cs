// Placeholder for Extensions/BusinessExtensions.cs
using CareFusion.Business.Services;
using CareFusion.Business.Validators;
using CareFusion.Model.Dtos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CareFusion.Business.Extensions;

public static class BusinessExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<PatientManager>();
        services.AddScoped<ExamManager>();
        services.AddScoped<AuthManager>();

        // Validators
        services.AddScoped<IValidator<PatientDto>, PatientValidator>();
        services.AddScoped<IValidator<ExamDto>, ExamValidator>();

        return services;
    }
}
