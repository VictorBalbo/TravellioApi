using TravellioApi.Models.Entities;

namespace TravellioApi.Models.DTOs;

public class DestinationDto
{
    public Guid? Id { get; init; }
    public required string PlaceId { get; init; }
    public required string Name { get; set; }
    public required Coordinates Coordinates { get; set; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public string? Notes { get; init; }
    public Place? Place { get; set; }
    public int? AccommodationsCount { get; init; }
    public ICollection<AccommodationDto>? Accommodations { get; init; }
    public int? ActivitiesCount { get; init; }
    public ICollection<ActivityDto>? Activities { get; init; }
}

public static class DestinationMapper
{
    extension(DestinationDto dto)
    {
        public Destination ToEntity(Guid tripId) => new()
        {
            Id = dto.Id ?? Guid.Empty,
            PlaceId = dto.PlaceId,
            Name =  dto.Name,
            Coordinates =  dto.Coordinates,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Notes = dto.Notes,
            Accommodations = dto.Accommodations?.ToEntity(dto.Id ?? Guid.Empty),
            Activities = dto.Activities?.ToEntity(dto.Id ?? Guid.Empty),
            TripId = tripId,
        };
    }

    extension(ICollection<DestinationDto> entities)
    {
        public ICollection<Destination> ToEntity(Guid tripId) => entities.Select((e) => e.ToEntity(tripId)).ToList();
    }

    extension(Destination entity)
    {
        public DestinationDto ToDto() => new()
        {
            Id = entity.Id,
            PlaceId = entity.PlaceId,
            Name = entity.Name,
            Coordinates = entity.Coordinates,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Notes = entity.Notes,
            Accommodations = entity.Accommodations?.ToDto(),
            AccommodationsCount =  entity.Accommodations?.Count,
            Activities = entity.Activities?.ToDto(),
            ActivitiesCount = entity.Activities?.Count,
        };
    }

    extension(ICollection<Destination> entities)
    {
        public ICollection<DestinationDto> ToDto() => entities.Select((e) => e.ToDto()).ToList();
    }
}