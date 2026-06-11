using TravellioApi.Models.Entities;

namespace TravellioApi.Models.DTOs;

public record PriceDto(decimal Value, string Currency)
{ }

public static class PriceMapper
{
    extension(PriceDto dto)
    {
        public Price ToEntity() => new()
        {
            Value = dto.Value,
            Currency = dto.Currency
        };
    }

    extension(Price entity)
    {
        public PriceDto ToDto() => new(entity.Value, entity.Currency);
    }
}