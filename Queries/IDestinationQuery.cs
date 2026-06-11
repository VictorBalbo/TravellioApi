using TravellioApi.Models.DTOs;

namespace TravellioApi.Queries;

public interface IDestinationQuery
{
    Task<ICollection<DestinationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<DestinationDto?> GetByIdAsync(Guid tripId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}