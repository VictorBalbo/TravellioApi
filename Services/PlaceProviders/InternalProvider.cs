using System.Text.Json;
using StackExchange.Redis;
using TravellioApi.Models;

namespace TravellioApi.Services.PlaceProviders;

public class InternalProvider(IConnectionMultiplexer redis) : ICachedPlaceProvider
{
    private readonly IDatabase _redis = redis.GetDatabase();
    private readonly TimeSpan _maxWaitTime = TimeSpan.FromSeconds(3);
    private const string PlaceKeyPrefix = "place";
    private readonly TimeSpan _defaultTtl = TimeSpan.FromDays(7);
    private const int JitterMaxHours = 168; // 7 days
    

    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_maxWaitTime);

            var placeKey = $"{PlaceKeyPrefix}:{placeId}";
            var value = await _redis.StringGetAsync(placeKey).WaitAsync(cts.Token);

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
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_maxWaitTime);
            
            var random = Random.Shared;
            var jitter = TimeSpan.FromHours(random.Next(0, JitterMaxHours));
            var ttl = _defaultTtl + jitter;

            var placeKey = $"{PlaceKeyPrefix}:{place.Id}";
            var placeJson = JsonSerializer.Serialize(place);
            return await _redis.StringSetAsync(placeKey, placeJson, ttl).WaitAsync(cts.Token);
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
}