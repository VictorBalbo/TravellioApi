using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface IDestinationService
{
    Task<ICollection<DestinationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<DestinationDto?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken);
    Task<DestinationDto> AddOrUpdateAsync(DestinationDto dto, Guid tripId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}