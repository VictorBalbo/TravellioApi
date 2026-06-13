using Travellio.Domain.DTOs;

namespace Travellio.Api.Queries;

public interface ITripQuery
{
    Task<ICollection<TripDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<TripDto?> GetByIdAsync(Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}