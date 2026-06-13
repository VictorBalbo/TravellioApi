using Travellio.Domain.DTOs;

namespace Travellio.Api.Queries;

public interface ITransportationQuery
{
    Task<ICollection<TransportationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<TransportationDto?> GetByIdAsync(Guid tripId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}