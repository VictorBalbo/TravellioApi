using Travellio.Domain.DTOs;

namespace Travellio.Api.Services.PlaceProviders;

public class NoCacheProvider : ICachedPlaceProvider
{
    public Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken) =>
        Task.FromResult<Place?>(null);

    public Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken) =>
        Task.FromResult(false);

    public Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(
        string text,
        string sessionToken,
        double lat,
        double lng,
        double radius,
        string language,
        CancellationToken cancellationToken) => Task.FromResult<IEnumerable<AutoComplete>?>(null);

    public Task<bool> SetAutoCompleteAsync(
        IEnumerable<AutoComplete> autoCompletes,
        string text,
        string language,
        CancellationToken cancellationToken) => Task.FromResult(false);
}