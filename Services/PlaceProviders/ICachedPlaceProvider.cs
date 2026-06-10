using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;

namespace TravellioApi.Services.PlaceProviders;

public interface ICachedPlaceProvider : IPlaceProvider
{
    Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken);

    Task<bool> SetAutoCompleteAsync(
        IEnumerable<AutoComplete> autoCompletes,
        string text,
        string language,
        CancellationToken cancellationToken);
}