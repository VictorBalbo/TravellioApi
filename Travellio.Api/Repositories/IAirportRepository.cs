namespace Travellio.Api.Repositories;

public interface IAirportRepository
{
    public Task<string?> GetIataCodeByCoordinatesAsync(decimal lat, decimal lng);
}