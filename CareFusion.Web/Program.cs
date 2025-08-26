// Placeholder for Program.cs
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services;
using CareFusion.Web.Services.Interfaces;
using CareFusion.Web.Data;
using CareFusion.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
builder.Services.AddDbContext<CareFusionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Authentication services
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, CareFusion.Web.Services.CustomAuthenticationStateProvider>();

// HttpClient for API
builder.Services.AddHttpClient("Api", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(cfg["Api:BaseUrl"]!);
});

// Typed services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// App state
builder.Services.AddScoped<AppState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
