namespace AdsetManagement.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> UpdateAsync(T entity);
    Task<bool> RemoveAsync(int id);
}