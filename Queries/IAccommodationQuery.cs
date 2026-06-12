using TravellioApi.Models.DTOs;

namespace TravellioApi.Queries;

public interface IAccommodationQuery
{
    Task<ICollection<AccommodationDto>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
    Task<AccommodationDto?> GetByIdAsync(Guid destinationId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}
