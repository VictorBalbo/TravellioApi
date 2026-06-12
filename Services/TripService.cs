using TravellioApi.Models.DTOs;
using TravellioApi.Repositories;

namespace TravellioApi.Services;

public class TripService(ITripRepository tripRepository) : ITripService
{
    public async Task<TripDto> AddOrUpdateAsync(TripDto dto, CancellationToken cancellationToken)
    {
        var trip = dto.ToEntity();
        await tripRepository.AddOrUpdateAsync(trip, cancellationToken);
        return trip.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await tripRepository.DeleteAsync(id, cancellationToken);
    }
}