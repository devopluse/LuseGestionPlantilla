using Serilog;
using LuseGestion.Infrastructure.Context;
using LuseGestion.Infrastructure.Repositories;
using LuseGestion.Application.Services;
using LuseGestion.Domain.Interfaces;
using Microsoft.OpenApi.Models;

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

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LuseGestion API",
        Version = "v1",
        Description = "API para la gestión de usuarios de LuseGestion",
        Contact = new OpenApiContact
        {
            Name = "LuseGestion Team",
            Email = "contact@lusegestion.com"
        }
    });

    // Habilitar anotaciones de Swagger
    options.EnableAnnotations();
});

// Add FluentValidation
// Note: AddFluentValidationAutoValidation removed - FluentValidation now integrated via DependencyInjectionExtensions

// Database connection
var connectionString = builder.Configuration.GetConnectionString("MySql");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'MySql' not found in configuration.");
}

builder.Services.AddSingleton<IMySqlConnectionFactory>(new MySqlConnectionFactory(connectionString));

// Register repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Register application services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LuseGestion API v1");
        options.RoutePrefix = string.Empty; // Swagger UI en la raíz (https://localhost:59160/)
        options.DocumentTitle = "LuseGestion API Documentation";
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
