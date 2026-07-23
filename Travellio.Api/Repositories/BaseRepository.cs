using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class, IBaseEntity
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
            var entry = Context.Entry(existingEntity);
            entry.CurrentValues.SetValues(entity);

            // CurrentValues.SetValues only copies scalar properties declared directly on the entity;
            // owned-type navigations (Price, Coordinates, ...) are ignored and must be applied manually.
            foreach (var reference in entry.References.Where(r => r.Metadata.TargetEntityType.IsOwned()))
            {
                var incomingValue = reference.Metadata.PropertyInfo!.GetValue(entity);
                if (incomingValue is null)
                    reference.CurrentValue = null;
                else if (reference.CurrentValue is null)
                    reference.CurrentValue = incomingValue;
                else
                    reference.TargetEntry!.CurrentValues.SetValues(incomingValue);
            }
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deletedEntities = await DbSet.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deletedEntities > 0;
    }
}