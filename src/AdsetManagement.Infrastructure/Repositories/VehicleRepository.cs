using Microsoft.EntityFrameworkCore;
using AdsetManagement.Domain.Entities;
using AdsetManagement.Domain.Interfaces;
using AdsetManagement.Infrastructure.Data;

namespace AdsetManagement.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleDbContext _context;
    
    public VehicleRepository(VehicleDbContext context)
    {
        _context = context;
    }
    
    public async Task<Vehicle> Add(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }
    
    public async Task<Vehicle?> GetById(int id)
    {
        return await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
    }
    
    public async Task<IEnumerable<Vehicle>> GetAll()
    {
        return await _context.Vehicles.ToListAsync();
    }
    
    public async Task<Vehicle?> Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }
    
    public async Task<bool> Remove(int id)
    {
        var vehicle = await GetById(id);
        if (vehicle == null) return false;
        
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<IEnumerable<Vehicle>> FindByFilters(string? marca, string? modelo, string? ano, string? cor, decimal? precoMin, decimal? precoMax, int? kmMax)
    {
        var vehicles = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(marca))
            vehicles = vehicles.Where(v => v.Marca.Contains(marca));

        if (!string.IsNullOrWhiteSpace(modelo))
            vehicles = vehicles.Where(v => v.Modelo.Contains(modelo));

        if (!string.IsNullOrWhiteSpace(ano))
            vehicles = vehicles.Where(v => v.Ano == ano);

        if (!string.IsNullOrWhiteSpace(cor))
            vehicles = vehicles.Where(v => v.Cor.Contains(cor));

        if (precoMin.HasValue)
            vehicles = vehicles.Where(v => v.Preco >= precoMin.Value);

        if (precoMax.HasValue)
            vehicles = vehicles.Where(v => v.Preco <= precoMax.Value);

        if (kmMax.HasValue)
            vehicles = vehicles.Where(v => v.Km <= kmMax.Value);

        return await vehicles.ToListAsync();
    }
}