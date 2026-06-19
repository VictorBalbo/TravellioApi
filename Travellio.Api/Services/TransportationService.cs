using Travellio.Api.Repositories;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;

namespace Travellio.Api.Services;

public class TransportationService(
    ITransportationRepository transportationRepository,
    IAirportRepository airportRepository) : ITransportationService
{
    private const int IataCodeLength = 3;

    public async Task<TransportationDto> AddOrUpdateAsync(TransportationDto dto, Guid tripId,
        CancellationToken cancellationToken)
    {
        var transportation = dto.ToEntity(tripId);

        foreach (var leg in transportation.Legs.Where(l => l.Type == TransportationType.Plane))
        {
            if (leg.DeparturePlaceShortName.Length > IataCodeLength)
            {
                var departure = await airportRepository.GetIataCodeByCoordinatesAsync(leg.DeparturePlaceCoordinates);
                if (departure is not null)
                {
                    leg.DeparturePlaceShortName = departure;
                }
            }

            if (leg.ArrivalPlaceShortName.Length > IataCodeLength)
            {
                var arrival = await airportRepository.GetIataCodeByCoordinatesAsync(leg.ArrivalPlaceCoordinates);
                if (arrival is not null)
                {
                    leg.ArrivalPlaceShortName = arrival;
                }
            }
        }

        await transportationRepository.AddOrUpdateAsync(transportation, cancellationToken);
        return transportation.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await transportationRepository.DeleteAsync(id, cancellationToken);
    }
}