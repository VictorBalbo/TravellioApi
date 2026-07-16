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
                HomePlaceName = t.HomePlaceName,
                ImageUrl = t.ImageUrl,
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
                HomePlaceName = t.HomePlaceName,
                ImageUrl = t.ImageUrl,
                Destinations = t.Destinations!.Select(d => new DestinationDto
                {
                    Id = d.Id,
                    PlaceId = d.PlaceId,
                    Name = d.Name,
                    Coordinates = d.Coordinates,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    Notes = d.Notes,
                    ImageUrl = d.ImageUrl,
                    ActivitiesCount = d.Activities!.Count,
                    AccommodationsCount = d.Accommodations!.Count,
                    Activities = d.Activities!.Select(a => new ActivityDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        PlaceId = a.PlaceId,
                        Address = a.Address,
                        Coordinates = a.Coordinates,
                        ScheduledAt = a.ScheduledAt,
                        TicketRequired = a.TicketRequired,
                        TicketPurchased = a.TicketPurchased,
                        Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
                        Website = a.Website,
                        Notes = a.Notes,
                        DestinationId = a.DestinationId,
                    }).ToList(),
                    Accommodations = d.Accommodations!.Select(a => new AccommodationDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Address = a.Address,
                        Coordinates = a.Coordinates,
                        PlaceId = a.PlaceId,
                        CheckIn = a.CheckIn,
                        CheckOut = a.CheckOut,
                        ImageUrl = a.ImageUrl,
                        Website = a.Website,
                        Notes = a.Notes,
                        Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
                        DestinationId = a.DestinationId,
                    }).ToList(),
                    TripId = d.TripId
                }).OrderBy(d => d.StartDate).ToList(),
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
                        DeparturePlaceDescription = l.DeparturePlaceDescription,
                        DeparturePlaceCoordinates = l.DeparturePlaceCoordinates,
                        ArrivalPlaceId = l.ArrivalPlaceId,
                        ArrivalPlaceShortName = l.ArrivalPlaceShortName,
                        ArrivalPlaceDescription = l.ArrivalPlaceDescription,
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
                    TripId =  tr.TripId,
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