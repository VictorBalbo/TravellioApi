using Travellio.Api.Services.PlaceProviders;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;

namespace Travellio.IntegrationTests.Mocks;

public class FakeWanderlogProvider : IPlaceProvider
{
    public string ProviderName { get; } = nameof(FakeWanderlogProvider);
    public int Priority { get; } = 1;

    private static readonly Place FakePlace = new()
    {
        Id = "place-paris",
        Name = "Paris",
        Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
        Address = "Paris, France",
        PhoneNumber = "+33 1 99 99 99 99",
    };

    public Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
        => Task.FromResult<Place?>(FakePlace);

    public Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(string text, string sessionToken, double lat,
        double lng, double radius, string language, string locationType, CancellationToken cancellationToken)
        => Task.FromResult<IEnumerable<AutoComplete>?>(null);

    public Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(string text, string sessionToken, double lat,
        double lng, double radius, string language, CancellationToken cancellationToken)
        => Task.FromResult<IEnumerable<AutoComplete>?>(null);
}