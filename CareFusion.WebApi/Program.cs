// Placeholder for Program.cs
using CareFusion.Business.Extensions;
using CareFusion.Core.Extensions;
using CareFusion.WebApi.Mappings;
using CareFusion.WebApi.Services;
using CareFusion.WebApi.Services.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CareFusion API", Version = "v1" });
});

builder.Services.AddAutoMapper(typeof(EntityToDtoProfile).Assembly);

builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IExamService, ExamService>();

// Infrastructure
builder.Services.AddCoreServices(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);
builder.Services.AddBusinessServices();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
