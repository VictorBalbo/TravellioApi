using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface ITransportationService
{
    Task<ICollection<TransportationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<TransportationDto?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken);
    Task<TransportationDto> AddOrUpdateAsync(TransportationDto dto, Guid tripId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}