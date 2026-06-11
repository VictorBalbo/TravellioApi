using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;
using TravellioApi.Repositories;

namespace TravellioApi.Services;

public class DestinationService(IDestinationRepository destinationRepository, IPlaceService placeService)
    : IDestinationService
{
    public async Task<ICollection<DestinationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        var destinations = await destinationRepository.GetAllAsync(tripId, cancellationToken);
        return destinations.ToDto();
    }

    public async Task<DestinationDto?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var destination = await destinationRepository.GetByIdAsync(tripId, id, cancellationToken);
        if (destination == null) return null;

        return await EnrichPlaceDetailsAsync(destination, cancellationToken);
    }

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

    private async Task<DestinationDto> EnrichPlaceDetailsAsync(Destination destination,
        CancellationToken cancellationToken)
    {
        var destinationDto = destination.ToDto();
        var destinationPlaceTask = placeService.GetPlaceDetails(destinationDto.PlaceId, cancellationToken);

        var accommodationTasks = (destinationDto.Accommodations ?? [])
            .Select(async a => a.Place = await placeService.GetPlaceDetails(a.PlaceId, cancellationToken));

        var activityTasks = (destinationDto.Activities ?? [])
            .Select(async a => a.Place = await placeService.GetPlaceDetails(a.PlaceId, cancellationToken));

        await Task.WhenAll([destinationPlaceTask, ..accommodationTasks, ..activityTasks]);

        return destinationDto;
    }
}