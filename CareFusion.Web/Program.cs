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

// Web app connects to WebAPI, no direct database connection needed

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Authentication services  
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, CareFusion.Web.Services.CustomAuthenticationStateProvider>();

// HttpClient for API
builder.Services.AddHttpClient("Api", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(cfg["Api:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        // Skip SSL certificate validation in development
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
    }
    return handler;
});

// Web application now calls WebAPI, no need for direct business/repository dependencies

// Typed services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IExamImageService, ExamImageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClinicSiteService, ClinicSiteService>();
builder.Services.AddScoped<IPatientNoteService, PatientNoteService>();
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

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
