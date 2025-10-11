using System.Linq.Expressions;
using CarbonTrackerApi.Data;
using CarbonTrackerApi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarbonTrackerApi.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.SingleOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await DbSet.ToListAsync();
        return await DbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? expression = null)
    {
        if (expression != null)
            return await DbSet.CountAsync(expression);
        return await DbSet.CountAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate != null)
            return await DbSet.CountAsync(predicate) > 0;
        return await DbSet.AnyAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }
}