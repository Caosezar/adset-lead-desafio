using AdsetManagement.Domain.Entities;

namespace AdsetManagement.Domain.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IEnumerable<Vehicle>> FindByFiltersAsync(string? marca, string? modelo, string? ano, string? cor, decimal? precoMin, decimal? precoMax, int? kmMax);
}