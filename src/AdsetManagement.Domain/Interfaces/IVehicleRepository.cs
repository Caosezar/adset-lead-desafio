using AdsetManagement.Domain.Entities;

namespace AdsetManagement.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle> Add(Vehicle vehicle);
    Task<Vehicle?> GetById(int id);
    Task<IEnumerable<Vehicle>> GetAll();
    Task<Vehicle?> Update(Vehicle vehicle);
    Task<bool> Remove(int id);
    Task<IEnumerable<Vehicle>> FindByFilters(string? marca, string? modelo, string? ano, string? cor, decimal? precoMin, decimal? precoMax, int? kmMax);
}