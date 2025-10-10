using Microsoft.EntityFrameworkCore;
using AdsetManagement.Infrastructure.Data;
using AdsetManagement.Domain.Interfaces;
using AdsetManagement.Infrastructure.Repositories;
using AdsetManagement.Infrastructure.Services;
using AdsetManagement.Application.Interfaces;
using AdsetManagement.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

try
{
    builder.Services.AddDbContext<VehicleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao configurar Entity Framework: {ex.Message}");
}

builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleImageRepository, VehicleImageRepository>();

builder.Services.AddScoped<IFileService>(provider =>
{
    try
    {
        var environment = provider.GetRequiredService<IWebHostEnvironment>();
        var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        
        if (!Directory.Exists(webRootPath))
            Directory.CreateDirectory(webRootPath);
            
        return new FileService(webRootPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao configurar FileService: {ex.Message}");
        var tempPath = Path.Combine(Path.GetTempPath(), "AdsetManagement", "uploads");
        if (!Directory.Exists(tempPath))
            Directory.CreateDirectory(tempPath);
        return new FileService(tempPath);
    }
});

// Application Services
builder.Services.AddScoped<IVehicleImageService, VehicleImageService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

try
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();

    Console.WriteLine("Aplica��o configurada com sucesso. Iniciando...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro na inicializa��o da aplica��o: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
    throw;
}