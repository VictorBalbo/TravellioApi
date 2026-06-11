using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;
using TravellioApi.Repositories;

namespace TravellioApi.Services;

public class TripService(ITripRepository tripRepository, IPlaceService placeService) : ITripService
{
    public async Task<ICollection<TripDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var trips = await tripRepository.GetAllAsync(cancellationToken);
        return trips.ToDto();
    }

    public async Task<TripDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var trip = await tripRepository.GetByIdAsync(id, cancellationToken);
        if (trip == null) return null;

        return await EnrichPlaceDetailsAsync(trip, cancellationToken);
    }

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

    private async Task<TripDto> EnrichPlaceDetailsAsync(Trip trip, CancellationToken cancellationToken)
    {
        var tripDto = trip.ToDto();
        var destinationTasks = (tripDto.Destinations ?? [])
            .Select(async d => d.Place = await placeService.GetPlaceDetails(d.PlaceId, cancellationToken));


        var legTasks = (tripDto.Transportations ?? [])
            .SelectMany(t => t.Legs)
            .Select(async l =>
            {
                var departureTerminalTask = placeService.GetPlaceDetails(l.DeparturePlaceId, cancellationToken);
                var arrivalTerminalTask = placeService.GetPlaceDetails(l.ArrivalPlaceId, cancellationToken);
                await Task.WhenAll(departureTerminalTask, arrivalTerminalTask);
                l.DeparturePlace = await departureTerminalTask;
                l.ArrivalPlace = await arrivalTerminalTask;
            });

        await Task.WhenAll([..destinationTasks, ..legTasks]);

        return tripDto;
    }
}