using Travellio.Api.Repositories;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public class TransportationService(ITransportationRepository transportationRepository) : ITransportationService
{
    public async Task<TransportationDto> AddOrUpdateAsync(TransportationDto dto, Guid tripId,
        CancellationToken cancellationToken)
    {
        var transportation = dto.ToEntity(tripId);
        await transportationRepository.AddOrUpdateAsync(transportation, cancellationToken);
        return transportation.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await transportationRepository.DeleteAsync(id, cancellationToken);
    }
}