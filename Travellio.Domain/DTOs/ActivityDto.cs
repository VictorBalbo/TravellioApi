using Travellio.Domain.Entities;

namespace Travellio.Domain.DTOs;

public class ActivityDto
{
    public Guid? Id { get; init; }
    public required string Name { get; init; }
    public required string PlaceId { get; init; }
    public required Coordinates Coordinates { get; set; }
    public string? Address { get; set; }
    public DateTime? ScheduledAt { get; init; }
    public bool? TicketRequired { get; init; }
    public bool? TicketPurchased { get; init; }
    public PriceDto? Price { get; init; }
    public string? Website { get; init; }
    public string? Notes { get; init; }
    public Place? Place { get; set; }
    public Guid? DestinationId { get; init; }
}

public static class ActivityMapper
{
    extension(ActivityDto dto)
    {
        public Activity ToEntity(Guid destinationId) => new()
        {
            Id = dto.Id ?? Guid.Empty,
            Name = dto.Name,
            PlaceId = dto.PlaceId,
            Coordinates = dto.Coordinates,
            Address = dto.Address,
            ScheduledAt = dto.ScheduledAt,
            TicketRequired = dto.TicketRequired,
            TicketPurchased = dto.TicketPurchased,
            Price = dto.Price?.ToEntity(),
            Website = dto.Website,
            Notes = dto.Notes,
            DestinationId = destinationId,
        };
    }

    extension(ICollection<ActivityDto> entities)
    {
        public ICollection<Activity> ToEntity(Guid tripId) => entities.Select((e) => e.ToEntity(tripId)).ToList();
    }

    extension(Activity entity)
    {
        public ActivityDto ToDto() => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            PlaceId = entity.PlaceId,
            Coordinates =  entity.Coordinates,
            Address = entity.Address,
            ScheduledAt = entity.ScheduledAt,
            TicketRequired = entity.TicketRequired,
            TicketPurchased = entity.TicketPurchased,
            Price = entity.Price?.ToDto(),
            Website = entity.Website,
            Notes = entity.Notes,
            DestinationId = entity.DestinationId,
        };
    }

    extension(ICollection<Activity> entities)
    {
        public ICollection<ActivityDto> ToDto() => entities.Select((e) => e.ToDto()).ToList();
    }
}