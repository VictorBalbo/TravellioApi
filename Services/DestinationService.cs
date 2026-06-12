using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;
using TravellioApi.Repositories;

namespace TravellioApi.Services;

public class DestinationService(IDestinationRepository destinationRepository, IPlaceService placeService)
    : IDestinationService
{
    public async Task<DestinationDto> AddOrUpdateAsync(DestinationDto dto, Guid tripId,
        CancellationToken cancellationToken)
    {
        var destination = dto.ToEntity(tripId);
        await destinationRepository.AddOrUpdateAsync(destination, cancellationToken);
        return destination.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await destinationRepository.DeleteAsync(id, cancellationToken);
    }
}