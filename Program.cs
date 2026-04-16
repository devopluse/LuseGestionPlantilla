using Microsoft.OpenApi.Models;
using Serilog;
using MyBackendApi.Infrastructure.Context;
using MyBackendApi.Infrastructure.Repositories;
using MyBackendApi.Application.Services;
using MyBackendApi.Domain.Interfaces;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "MyBackendApi", 
        Version = "v1",
        Description = "API RESTful con arquitectura en capas"
    });
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("MySql");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'MySql' not found in configuration.");
}

builder.Services.AddSingleton<IMySqlConnectionFactory>(new MySqlConnectionFactory(connectionString));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register application services
builder.Services.AddScoped<IUserService, UserService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyBackendApi v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
