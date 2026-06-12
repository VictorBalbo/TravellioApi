using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface ITransportationService
{
    Task<TransportationDto> AddOrUpdateAsync(TransportationDto dto, Guid tripId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}