using Microsoft.EntityFrameworkCore;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Queries;

public class TripQuery(AppDbContext context, IPlaceService placeService) : ITripQuery
{
    private readonly DbSet<Trip> _dbSet = context.Set<Trip>();

    public async Task<ICollection<TripDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var trips = await _dbSet
            .AsNoTracking()
            .Select(t => new TripDto
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                HomePlaceId = t.HomePlaceId,
            })
            .ToListAsync(cancellationToken);
        return trips;
    }

    public async Task<TripDto?> GetByIdAsync(Guid id, bool enrichPlaces, CancellationToken cancellationToken)
    {
        var trip = await _dbSet
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TripDto
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                HomePlaceId = t.HomePlaceId,
                Destinations = t.Destinations!.Select(d => new DestinationDto
                {
                    Id = d.Id,
                    PlaceId = d.PlaceId,
                    Name =  d.Name,
                    Coordinates =  d.Coordinates,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    Notes = d.Notes,
                    ActivitiesCount = d.Activities!.Count,
                    AccommodationsCount = d.Accommodations!.Count,
                }).ToList(),
                Transportations = t.Transportations!.Select(tr => new TransportationDto
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
                }).ToList(),
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (trip != null && enrichPlaces)
        {
            trip = await EnrichPlaceDetailsAsync(trip, cancellationToken);
        }

        return trip;
    }

    private async Task<TripDto> EnrichPlaceDetailsAsync(TripDto trip, CancellationToken cancellationToken)
    {
        var destinationTasks = (trip.Destinations ?? [])
            .Select(async d => d.Place = await placeService.GetPlaceDetails(d.PlaceId, cancellationToken));


        var legTasks = (trip.Transportations ?? [])
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

        return trip;
    }
}