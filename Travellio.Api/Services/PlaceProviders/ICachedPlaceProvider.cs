using Travellio.Domain.DTOs;

namespace Travellio.Api.Services.PlaceProviders;

public interface ICachedPlaceProvider : IPlaceProvider
{
    Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken);

    Task<bool> SetAutoCompleteAsync(
        IEnumerable<AutoComplete> autoCompletes,
        string text,
        string language,
        CancellationToken cancellationToken);
}