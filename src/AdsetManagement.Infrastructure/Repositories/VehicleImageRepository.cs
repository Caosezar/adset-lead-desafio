using Microsoft.EntityFrameworkCore;
using AdsetManagement.Domain.Entities;
using AdsetManagement.Domain.Interfaces;
using AdsetManagement.Infrastructure.Data;

namespace AdsetManagement.Infrastructure.Repositories;

public class VehicleImageRepository : IVehicleImageRepository
{
    private readonly VehicleDbContext _context;

    public VehicleImageRepository(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<bool> VehicleExistsAsync(int vehicleId)
    {
        return await _context.Vehicles.AnyAsync(v => v.Id == vehicleId);
    }

    public async Task<int> GetImageCountAsync(int vehicleId)
    {
        return await _context.VehicleImages.CountAsync(i => i.VehicleId == vehicleId);
    }

    public async Task<VehicleImage> AddImageAsync(VehicleImage vehicleImage)
    {
        _context.VehicleImages.Add(vehicleImage);
        return vehicleImage;
    }

    public async Task<List<VehicleImage>> GetImagesByVehicleIdAsync(int vehicleId)
    {
        return await _context.VehicleImages
            .Where(i => i.VehicleId == vehicleId)
            .ToListAsync();
    }

    public async Task<VehicleImage?> GetImageByIdAsync(int vehicleId, int imageId)
    {
        return await _context.VehicleImages
            .FirstOrDefaultAsync(i => i.Id == imageId && i.VehicleId == vehicleId);
    }

    public async Task<bool> DeleteImageAsync(VehicleImage vehicleImage)
    {
        _context.VehicleImages.Remove(vehicleImage);
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}