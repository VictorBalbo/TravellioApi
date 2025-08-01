
using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;

namespace Travellio.Repository;

public class BaseRepository<T> : IRepository<T> where T : class, IModel
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task AddOrUpdateAsync(T entity)
    {
        var existingEntity = await _dbSet.FindAsync(entity.Id);

        if (existingEntity == null)
        {
            await _dbSet.AddAsync(entity);
        }
        else
        {
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
