using Travellio.Domain.Entities;

namespace Travellio.Domain.DTOs;

public class TripDto
{
    public Guid? Id { get; init; }
    public required string Name { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public string? HomePlaceId { get; init; }
    public string? HomePlaceName { get; set; }
    public ICollection<DestinationDto>? Destinations { get; init; }
    public ICollection<TransportationDto>? Transportations { get; init; }
}

public static class TripMapper
{
    extension(TripDto dto)
    {
        public Trip ToEntity() => new()
        {
            Id = dto.Id ?? Guid.Empty,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            HomePlaceId = dto.HomePlaceId,
            HomePlaceName = dto.HomePlaceName,
            Destinations = dto.Destinations?.ToEntity(dto.Id ?? Guid.Empty),
            Transportations = dto.Transportations?.ToEntity(dto.Id ?? Guid.Empty),
        };
    }

    extension(ICollection<TripDto> entities)
    {
        public ICollection<Trip> ToEntity(Guid tripId) => entities.Select(e => e.ToEntity()).ToList();
    }

    extension(Trip entity)
    {
        public TripDto ToDto() => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            HomePlaceId = entity.HomePlaceId,
            HomePlaceName = entity.HomePlaceName,
            Destinations = entity.Destinations?.ToDto(),
            Transportations = entity.Transportations?.ToDto(),
        };
    }

    extension(ICollection<Trip> entities)
    {
        public ICollection<TripDto> ToDto() => entities.Select((e) => e.ToDto()).ToList();
    }
}