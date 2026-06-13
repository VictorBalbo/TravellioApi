using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public interface IDestinationService
{
    Task<DestinationDto> AddOrUpdateAsync(DestinationDto dto, Guid tripId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}