using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface ITripService
{
    Task<TripDto> AddOrUpdateAsync(TripDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}