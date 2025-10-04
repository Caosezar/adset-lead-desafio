namespace AdsetManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IVehicleRepository VehicleRepository { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}