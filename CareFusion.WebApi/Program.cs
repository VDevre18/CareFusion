using CareFusion.Business.Extensions;
using CareFusion.Core.Extensions;
using CareFusion.WebApi.Mappings;
using CareFusion.WebApi.Services;
using CareFusion.WebApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CareFusion API", Version = "v1" });
});

builder.Services.AddAutoMapper(typeof(EntityToDtoProfile).Assembly);

builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClinicSiteService, ClinicSiteService>();

// Infrastructure
builder.Services.AddCoreServices(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);
builder.Services.AddBusinessServices();

var app = builder.Build();

// Database initialization and seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CareFusion.Core.CareFusionDbContext>();
    
    // Ensure database is created and migrated
    await context.Database.MigrateAsync();
    
    // Seed initial data
    await SeedDatabaseAsync(context);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(); // Enable CORS
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task SeedDatabaseAsync(CareFusion.Core.CareFusionDbContext context)
{
    // Seed users if none exist
    if (!await context.Users.AnyAsync())
    {
        var users = new[]
        {
            new CareFusion.Core.Entities.User
            {
                Username = "admin",
                Email = "admin@carefusion.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                Role = "Admin",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.User
            {
                Username = "doctor",
                Email = "doctor@carefusion.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor123"),
                Role = "Doctor",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.User
            {
                Username = "nurse1",
                Email = "nurse1@carefusion.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("nurse123"),
                Role = "Nurse",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            }
        };

        await context.Users.AddRangeAsync(users);
    }

    // Seed patients if none exist
    if (!await context.Patients.AnyAsync())
    {
        var patients = new[]
        {
            new CareFusion.Core.Entities.Patient
            {
                FirstName = "John",
                LastName = "Doe",
                MRN = "MRN001",
                DateOfBirth = new DateTime(1980, 5, 15),
                Gender = "Male",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.Patient
            {
                FirstName = "Jane",
                LastName = "Smith",
                MRN = "MRN002",
                DateOfBirth = new DateTime(1975, 8, 22),
                Gender = "Female",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.Patient
            {
                FirstName = "Robert",
                LastName = "Johnson",
                MRN = "MRN003",
                DateOfBirth = new DateTime(1990, 2, 10),
                Gender = "Male",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.Patient
            {
                FirstName = "Sarah",
                LastName = "Williams",
                MRN = "MRN004",
                DateOfBirth = new DateTime(1985, 12, 3),
                Gender = "Female",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            }
        };

        await context.Patients.AddRangeAsync(patients);
    }

    // Seed clinic sites if none exist
    if (!await context.ClinicSites.AnyAsync())
    {
        var clinicSites = new[]
        {
            new CareFusion.Core.Entities.ClinicSite
            {
                Name = "Main Medical Center",
                Code = "MAIN",
                Address = "123 Healthcare Blvd",
                City = "Medical City",
                State = "CA",
                PostalCode = "90210",
                Country = "USA",
                Phone = "(555) 123-4567",
                Email = "main@carefusion.com",
                IsActive = true,
                Description = "Primary medical center with full diagnostic capabilities",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.ClinicSite
            {
                Name = "Downtown Clinic",
                Code = "DOWN",
                Address = "456 Central Ave",
                City = "Metro City",
                State = "CA",
                PostalCode = "90211",
                Country = "USA",
                Phone = "(555) 234-5678",
                Email = "downtown@carefusion.com",
                IsActive = true,
                Description = "Downtown location for convenient patient access",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            },
            new CareFusion.Core.Entities.ClinicSite
            {
                Name = "Suburban Branch",
                Code = "SUB",
                Address = "789 Suburban Dr",
                City = "Suburbs",
                State = "CA",
                PostalCode = "90212",
                Country = "USA",
                Phone = "(555) 345-6789",
                Email = "suburban@carefusion.com",
                IsActive = true,
                Description = "Suburban branch office for local community",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            }
        };

        await context.ClinicSites.AddRangeAsync(clinicSites);
    }

    await context.SaveChangesAsync("system");
}
