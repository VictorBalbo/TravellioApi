using System.Text.Json;
using StackExchange.Redis;
using TravellioApi.Models;

namespace TravellioApi.Services.PlaceProviders;

public class InternalProvider(IConnectionMultiplexer redis) : ICachedPlaceProvider
{
    private readonly IDatabase _redis = redis.GetDatabase();
    private readonly TimeSpan _maxWaitTime = TimeSpan.FromSeconds(3);

    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        try
        {
            var token = GetCancellationToken(cancellationToken);

            var placeKey = $"place:{placeId}";
            var value = await _redis.StringGetAsync(placeKey).WaitAsync(token);

            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Place>(value.ToString());
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Redis operation cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Redis operation error {e.Message}");
        }

        return null;
    }

    public async Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken)
    {
        try
        {
            var token = GetCancellationToken(cancellationToken);

            var placeKey = $"place:{place.Id}";
            var placeJson = JsonSerializer.Serialize(place);
            return await _redis.StringSetAsync(placeKey, placeJson).WaitAsync(token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Redis operation cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Redis operation error {e.Message}");
        }

        return false;
    }

    private CancellationToken GetCancellationToken(CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(_maxWaitTime);
        return linkedCts.Token;
    }
}