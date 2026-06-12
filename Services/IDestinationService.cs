using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface IDestinationService
{
    Task<DestinationDto> AddOrUpdateAsync(DestinationDto dto, Guid tripId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}