using TravellioApi.Models.DTOs;

namespace TravellioApi.Queries;

public interface ITransportationQuery
{
    Task<ICollection<TransportationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<TransportationDto?> GetByIdAsync(Guid tripId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}