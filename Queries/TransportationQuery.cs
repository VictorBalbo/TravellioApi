using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;
using TravellioApi.Services;

namespace TravellioApi.Queries;

public class TransportationQuery(AppDbContext context, IPlaceService placeService) : ITransportationQuery
{
    private readonly DbSet<Transportation> _dbSet = context.Set<Transportation>();

    public async Task<ICollection<TransportationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(tr => tr.TripId == tripId)
            .Select(tr => new TransportationDto
            {
                Id = tr.Id,
                Price = tr.Price == null ? null : new PriceDto(tr.Price.Value, tr.Price.Currency),
                ArrivalDestinationId = tr.ArrivalDestinationId,
                DepartureDestinationId = tr.DepartureDestinationId,
                Legs = tr.Legs.Select(l => new LegDto
                {
                    Id = l.Id,
                    DeparturePlaceId = l.DeparturePlaceId,
                    DeparturePlaceShortName = l.DeparturePlaceShortName,
                    DeparturePlaceDescription =  l.DeparturePlaceDescription,
                    DeparturePlaceCoordinates =  l.DeparturePlaceCoordinates,
                    ArrivalPlaceId = l.ArrivalPlaceId,
                    ArrivalPlaceShortName = l.ArrivalPlaceShortName,
                    ArrivalPlaceDescription =  l.ArrivalPlaceDescription,
                    ArrivalPlaceCoordinates = l.ArrivalPlaceCoordinates,
                    Type = l.Type,
                    DepartureTime = l.DepartureTime,
                    ArrivalTime = l.ArrivalTime,
                    Price = l.Price == null ? null : new PriceDto(l.Price.Value, l.Price.Currency),
                    Company = l.Company,
                    ServiceNumber = l.ServiceNumber,
                    Reservation = l.Reservation,
                    Seat = l.Seat,
                }).ToList(),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TransportationDto?> GetByIdAsync(Guid tripId, Guid id, bool enrichPlaces, CancellationToken cancellationToken)
    {
        var transportation = await _dbSet
            .AsNoTracking()
            .Where(tr => tr.TripId == tripId && tr.Id == id)
            .Select(tr => new TransportationDto
            {
                Id = tr.Id,
                Price = tr.Price == null ? null : new PriceDto(tr.Price.Value, tr.Price.Currency),
                ArrivalDestinationId = tr.ArrivalDestinationId,
                DepartureDestinationId = tr.DepartureDestinationId,
                Legs = tr.Legs.Select(l => new LegDto
                {
                    Id = l.Id,
                    DeparturePlaceId = l.DeparturePlaceId,
                    DeparturePlaceShortName = l.DeparturePlaceShortName,
                    DeparturePlaceDescription =  l.DeparturePlaceDescription,
                    DeparturePlaceCoordinates =  l.DeparturePlaceCoordinates,
                    ArrivalPlaceId = l.ArrivalPlaceId,
                    ArrivalPlaceShortName = l.ArrivalPlaceShortName,
                    ArrivalPlaceDescription =  l.ArrivalPlaceDescription,
                    ArrivalPlaceCoordinates = l.ArrivalPlaceCoordinates,
                    Type = l.Type,
                    DepartureTime = l.DepartureTime,
                    ArrivalTime = l.ArrivalTime,
                    Price = l.Price == null ? null : new PriceDto(l.Price.Value, l.Price.Currency),
                    Company = l.Company,
                    ServiceNumber = l.ServiceNumber,
                    Reservation = l.Reservation,
                    Seat = l.Seat,
                }).ToList(),
                Arrival = tr.Arrival == null ? null : new DestinationDto
                {
                    Id = tr.Arrival.Id,
                    PlaceId = tr.Arrival.PlaceId,
                    Name =  tr.Arrival.Name,
                    Coordinates =  tr.Arrival.Coordinates,
                    StartDate = tr.Arrival.StartDate,
                    EndDate = tr.Arrival.EndDate,
                    Notes = tr.Arrival.Notes,
                },
                Departure = tr.Departure == null ? null : new DestinationDto
                {
                    Id = tr.Departure.Id,
                    PlaceId = tr.Departure.PlaceId,
                    Name =  tr.Departure.Name,
                    Coordinates = tr.Departure.Coordinates,
                    StartDate = tr.Departure.StartDate,
                    EndDate = tr.Departure.EndDate,
                    Notes = tr.Departure.Notes,
                },
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (transportation != null && enrichPlaces)
        {
            transportation = await EnrichPlaceDetailsAsync(transportation, cancellationToken);
        }

        return transportation;
    }

    private async Task<TransportationDto> EnrichPlaceDetailsAsync(TransportationDto transportation, CancellationToken cancellationToken)
    {
        var legTasks = transportation.Legs
            .Select(async l =>
            {
                var departureTask = placeService.GetPlaceDetails(l.DeparturePlaceId, cancellationToken);
                var arrivalTask = placeService.GetPlaceDetails(l.ArrivalPlaceId, cancellationToken);
                await Task.WhenAll(departureTask, arrivalTask);
                l.DeparturePlace = await departureTask;
                l.ArrivalPlace = await arrivalTask;
            });

        var arrivalTask = transportation.Arrival != null
            ? placeService.GetPlaceDetails(transportation.Arrival.PlaceId, cancellationToken)
            : Task.FromResult<Place?>(null);

        var departureTask = transportation.Departure != null
            ? placeService.GetPlaceDetails(transportation.Departure.PlaceId, cancellationToken)
            : Task.FromResult<Place?>(null);

        await Task.WhenAll([arrivalTask, departureTask, ..legTasks]);

        transportation.Arrival?.Place = await arrivalTask;
        transportation.Departure?.Place = await departureTask;

        return transportation;
    }
}