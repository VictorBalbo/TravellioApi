using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface IAccommodationService
{
    Task<AccommodationDto> AddOrUpdateAsync(AccommodationDto dto, Guid destinationId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
