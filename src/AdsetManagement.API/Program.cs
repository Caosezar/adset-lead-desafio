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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<VehicleDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("AdsetManagement.Infrastructure");
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        }));

builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleImageRepository, VehicleImageRepository>();
builder.Services.AddScoped<IFileService>(provider =>
{
    var environment = provider.GetRequiredService<IWebHostEnvironment>();
    return new FileService(environment.WebRootPath);
});
builder.Services.AddScoped<IVehicleImageService>(provider =>
{
    var vehicleImageRepository = provider.GetRequiredService<IVehicleImageRepository>();
    var vehicleRepository = provider.GetRequiredService<IVehicleRepository>();
    var fileService = provider.GetRequiredService<IFileService>();
    return new VehicleImageService(vehicleImageRepository, vehicleRepository, fileService);
});
builder.Services.AddScoped<IVehicleService>(provider =>
{
    var vehicleRepository = provider.GetRequiredService<IVehicleRepository>();
    var vehicleImageRepository = provider.GetRequiredService<IVehicleImageRepository>();
    var vehicleImageService = provider.GetRequiredService<IVehicleImageService>();
    return new VehicleService(vehicleRepository, vehicleImageRepository, vehicleImageService);
});

var app = builder.Build();

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

app.Run();