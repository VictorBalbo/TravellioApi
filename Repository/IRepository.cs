using Travellio.Models;

namespace Travellio.Repository;

public interface IRepository<T> where T : class, IModel
{
    //Task<IEnumerable<T>> GetAllAsync();
    //Task<T?> GetByIdAsync(Guid id);
    Task AddOrUpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
