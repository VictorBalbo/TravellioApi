using Travellio.Domain.Entities;

namespace Travellio.Api.Repositories;

public interface IAirportRepository
{
    public Task<string?> GetIataCodeByCoordinatesAsync(Coordinates coordinates);
}