using Travellio.Domain.Entities;

namespace Travellio.Domain.DTOs;

public class TransportationDto
{
    public Guid? Id { get; init; }
    public PriceDto? Price { get; init; }
    public required ICollection<LegDto> Legs { get; init; }
    public Guid? ArrivalDestinationId { get; init; }
    public Guid? DepartureDestinationId { get; init; }
    public DateTime? DepartureTime => Legs.OrderBy(l => l.DepartureTime).FirstOrDefault()?.DepartureTime;
    public DateTime? ArrivalTime => Legs.OrderBy(l => l.DepartureTime).LastOrDefault()?.ArrivalTime;
    public DestinationDto? Arrival { get; init; }
    public DestinationDto? Departure { get; init; }
    public Guid? TripId { get; init; }
}

public static class TransportationMapper
{
    extension(TransportationDto dto)
    {
        public Transportation ToEntity(Guid tripId) => new()
        {
            Id = dto.Id ?? Guid.Empty,
            Price = dto.Price?.ToEntity(),
            Legs = dto.Legs.ToEntity(dto.Id ?? Guid.Empty),
            ArrivalDestinationId = dto.ArrivalDestinationId,
            DepartureDestinationId = dto.DepartureDestinationId,
            Arrival = dto.ArrivalDestinationId.HasValue
                ? dto.Arrival?.ToEntity(dto.ArrivalDestinationId.Value)
                : null,
            Departure = dto.DepartureDestinationId.HasValue
                ? dto.Departure?.ToEntity(dto.DepartureDestinationId.Value)
                : null,
            TripId = tripId,
        };
    }

    extension(ICollection<TransportationDto> entities)
    {
        public ICollection<Transportation> ToEntity(Guid tripId) =>
            entities.Select((e) => e.ToEntity(tripId)).ToList();
    }

    extension(Transportation entity)
    {
        public TransportationDto ToDto() => new()
        {
            Id = entity.Id,
            Price = entity.Price?.ToDto(),
            Legs = entity.Legs.ToDto(),
            ArrivalDestinationId = entity.ArrivalDestinationId,
            DepartureDestinationId = entity.DepartureDestinationId,
            Arrival = entity.Arrival?.ToDto(),
            Departure = entity.Departure?.ToDto(),
            TripId = entity.TripId,
        };
    }

    extension(ICollection<Transportation> entities)
    {
        public ICollection<TransportationDto> ToDto() => entities.Select((e) => e.ToDto()).ToList();
    }
}