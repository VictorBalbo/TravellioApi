using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;
using TravellioApi.Repositories;

namespace TravellioApi.Services;

public class TransportationService(ITransportationRepository transportationRepository, IPlaceService placeService)
    : ITransportationService
{
    public async Task<ICollection<TransportationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        var transportations = await transportationRepository.GetAllAsync(tripId, cancellationToken);
        return transportations.ToDto();
    }

    public async Task<TransportationDto?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var transportation = await transportationRepository.GetByIdAsync(tripId, id, cancellationToken);
        if (transportation == null) return null;

        return await EnrichPlaceDetailsAsync(transportation, cancellationToken);
    }

    public async Task<TransportationDto> AddOrUpdateAsync(TransportationDto dto, Guid tripId,
        CancellationToken cancellationToken)
    {
        var transportation = dto.ToEntity(tripId);
        await transportationRepository.AddOrUpdateAsync(transportation, cancellationToken);
        return transportation.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await transportationRepository.DeleteAsync(id, cancellationToken);
    }

    private async Task<TransportationDto> EnrichPlaceDetailsAsync(Transportation transportation,
        CancellationToken cancellationToken)
    {
        var transportationDto = transportation.ToDto();
        var tasks = new List<Task>();

        var arrivalPlaceTask = Task.FromResult<Place?>(null);
        var departurePlaceTask = Task.FromResult<Place?>(null);

        if (transportationDto.Arrival?.PlaceId != null)
        {
            arrivalPlaceTask = placeService.GetPlaceDetails(transportationDto.Arrival.PlaceId, cancellationToken);
            tasks.Add(arrivalPlaceTask);
        }

        if (transportationDto.Departure?.PlaceId != null)
        {
            departurePlaceTask = placeService.GetPlaceDetails(transportationDto.Departure.PlaceId, cancellationToken);
            tasks.Add(departurePlaceTask);
        }

        await Task.WhenAll(tasks);

        transportationDto.Arrival?.Place = await arrivalPlaceTask;
        transportationDto.Departure?.Place = await departurePlaceTask;

        return transportationDto; 
    }
}