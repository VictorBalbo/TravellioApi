using TravellioApi.Models.Entities;

namespace TravellioApi.Models.DTOs;

public class LegDto
{
    public Guid? Id { get; init; }
    public required string DeparturePlaceId { get; init; }
    public required string DeparturePlaceShortName { get; init; }
    public required string DeparturePlaceDescription { get; init; }
    public required Coordinates DeparturePlaceCoordinates { get; init; }
    public required string ArrivalPlaceId { get; init; }
    public required string ArrivalPlaceShortName { get; init; }
    public required string ArrivalPlaceDescription { get; init; }
    public required Coordinates ArrivalPlaceCoordinates { get; init; }
    public TransportationType Type { get; init; }
    public DateTime? DepartureTime { get; init; }
    public DateTime? ArrivalTime { get; init; }
    public PriceDto? Price { get; init; }
    public string? Company { get; init; }
    public string? ServiceNumber { get; init; }
    public string? Reservation { get; init; }
    public string? Seat { get; init; }
    public Place? DeparturePlace { get; set; }
    public Place? ArrivalPlace { get; set; }
}

public static class LegMapper
{
    extension(LegDto dto)
    {
        public Leg ToEntity(Guid transportationId) => new()
        {
            Id = dto.Id ?? Guid.Empty,
            DeparturePlaceId = dto.DeparturePlaceId,
            DeparturePlaceShortName = dto.DeparturePlaceShortName,
            DeparturePlaceDescription = dto.DeparturePlaceDescription,
            DeparturePlaceCoordinates = dto.DeparturePlaceCoordinates,
            ArrivalPlaceId = dto.ArrivalPlaceId,
            ArrivalPlaceShortName = dto.ArrivalPlaceShortName,
            ArrivalPlaceDescription = dto.ArrivalPlaceDescription,
            ArrivalPlaceCoordinates = dto.ArrivalPlaceCoordinates,
            Type = dto.Type,
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            Price = dto.Price?.ToEntity(),
            Company = dto.Company,
            ServiceNumber = dto.ServiceNumber,
            Reservation = dto.Reservation,
            Seat = dto.Seat,
            TransportationId = transportationId,
        };
    }

    extension(ICollection<LegDto> entities)
    {
        public ICollection<Leg> ToEntity(Guid tripId) => entities.Select((e) => e.ToEntity(tripId)).ToList();
    }

    extension(Leg entity)
    {
        public LegDto ToDto() => new()
        {
            Id = entity.Id,
            DeparturePlaceId = entity.DeparturePlaceId,
            DeparturePlaceShortName = entity.DeparturePlaceShortName,
            DeparturePlaceDescription = entity.DeparturePlaceDescription,
            DeparturePlaceCoordinates = entity.DeparturePlaceCoordinates,
            ArrivalPlaceId = entity.ArrivalPlaceId,
            ArrivalPlaceShortName = entity.ArrivalPlaceShortName,
            ArrivalPlaceDescription = entity.ArrivalPlaceDescription,
            ArrivalPlaceCoordinates = entity.ArrivalPlaceCoordinates,
            Type = entity.Type,
            DepartureTime = entity.DepartureTime,
            ArrivalTime = entity.ArrivalTime,
            Price = entity.Price?.ToDto(),
            Company = entity.Company,
            ServiceNumber = entity.ServiceNumber,
            Reservation = entity.Reservation,
            Seat = entity.Seat,
        };
    }

    extension(ICollection<Leg> entities)
    {
        public ICollection<LegDto> ToDto() => entities.Select((e) => e.ToDto()).ToList();
    }
}