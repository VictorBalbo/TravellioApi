using TravellioApi.Models.Entities;

namespace TravellioApi.Models.DTOs;

public class AccommodationDto
{
    public Guid? Id { get; init; }
    public required string Name { get; init; }
    public required string PlaceId { get; init; }
    public DateTime? CheckIn { get; init; }
    public DateTime? CheckOut { get; init; }
    public string? ImageUrl { get; init; }
    public string? Website { get; init; }
    public string? Notes { get; init; }
    public PriceDto? Price { get; init; }
    public Place? Place { get; set; }
}

public static class AccommodationMapper
{
    extension(AccommodationDto dto)
    {
        public Accommodation ToEntity(Guid destinationId) => new()
        {
            Id = dto.Id ?? Guid.Empty,
            Name = dto.Name,
            PlaceId = dto.PlaceId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            ImageUrl = dto.ImageUrl,
            Website = dto.Website,
            Notes = dto.Notes,
            Price = dto.Price?.ToEntity(),
            DestinationId = destinationId,
        };
    }

    extension(ICollection<AccommodationDto> entities)
    {
        public ICollection<Accommodation> ToEntity(Guid tripId) => entities.Select((e) => e.ToEntity(tripId)).ToList();
    }

    extension(Accommodation entity)
    {
        public AccommodationDto ToDto() => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            PlaceId = entity.PlaceId,
            CheckIn = entity.CheckIn,
            CheckOut = entity.CheckOut,
            ImageUrl = entity.ImageUrl,
            Website = entity.Website,
            Notes = entity.Notes,
            Price = entity.Price?.ToDto(),
        };
    }

    extension(ICollection<Accommodation> entities)
    {
        public ICollection<AccommodationDto> ToDto() => entities.Select((e) => e.ToDto()).ToList();
    }
}