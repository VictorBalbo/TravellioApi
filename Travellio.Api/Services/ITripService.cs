using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public interface ITripService
{
    Task<TripDto> AddOrUpdateAsync(TripDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}