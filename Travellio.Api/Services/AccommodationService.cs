using Travellio.Api.Repositories;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public class AccommodationService(IAccommodationRepository accommodationRepository) : IAccommodationService
{
    public async Task<AccommodationDto> AddOrUpdateAsync(AccommodationDto dto, Guid destinationId, CancellationToken cancellationToken)
    {
        var accommodation = dto.ToEntity(destinationId);
        await accommodationRepository.AddOrUpdateAsync(accommodation, cancellationToken);
        return accommodation.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await accommodationRepository.DeleteAsync(id, cancellationToken);
    }
}
