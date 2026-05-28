using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models;

namespace TravellioApi.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class, IModel
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(AppDbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return entity;
    }

    public virtual async Task AddOrUpdateAsync(T entity, CancellationToken cancellationToken)
    {
        var existingEntity = await DbSet.FindAsync([entity.Id], cancellationToken);
        if (existingEntity == null)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }
        else
        {
            Context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deletedEntities = await DbSet.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deletedEntities > 0;
    }
}