using System.Linq.Expressions;

namespace CarbonTrackerApi.Interfaces.Repositories;

public interface IRepository<T>
    where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? expression = null);
    Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null);
    Task<int> SaveChangesAsync();
}