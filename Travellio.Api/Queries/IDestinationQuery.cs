using Travellio.Domain.DTOs;

namespace Travellio.Api.Queries;

public interface IDestinationQuery
{
    Task<ICollection<DestinationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<DestinationDto?> GetByIdAsync(Guid tripId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}