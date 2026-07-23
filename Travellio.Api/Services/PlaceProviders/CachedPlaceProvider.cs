using System.Text.Json;
using StackExchange.Redis;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services.PlaceProviders;

public class CachedPlaceProvider(ILogger<CachedPlaceProvider> logger, IConnectionMultiplexer? redis = null)
    : ICachedPlaceProvider
{
    private readonly IDatabase? _redis = redis?.GetDatabase();
    private readonly TimeSpan _maxWaitTime = TimeSpan.FromSeconds(3);

    private const string PlaceKeyPrefix = "place";
    private readonly TimeSpan _placeDefaultTtl = TimeSpan.FromDays(7);
    private const int PlaceJitterMaxHours = 168; // 7 days

    private const string AutoCompleteKeyPrefix = "autocomplete";
    private readonly TimeSpan _autoCompleteDefaultTtl = TimeSpan.FromMinutes(60);
    private const int AutoCompleteJitterMaxMins = 60; // 1 hour


    public string ProviderName { get; } = nameof(CachedPlaceProvider);
    public int Priority { get; } = 0;

    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        if (_redis is null)
        {
            return null;
        }

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
            logger.LogWarning("Redis operation cancelled");
        }
        catch (Exception e)
        {
            logger.LogWarning("Redis operation error {Message} during {Operation}", e.Message,
                nameof(GetPlaceDetailsAsync));
        }

        return null;
    }

    public async Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken)
    {
        if (_redis is null)
        {
            return false;
        }

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_maxWaitTime);

            var random = Random.Shared;
            var jitter = TimeSpan.FromHours(random.Next(0, PlaceJitterMaxHours));
            var ttl = _placeDefaultTtl + jitter;

            var placeKey = $"{PlaceKeyPrefix}:{place.Id}";
            var placeJson = JsonSerializer.Serialize(place);
            return await _redis.StringSetAsync(placeKey, placeJson, ttl).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Redis operation cancelled");
        }
        catch (Exception e)
        {
            logger.LogWarning("Redis operation error {Message} during {Operation}", e.Message,
                nameof(SetPlaceDetailsAsync));
        }

        return false;
    }

    public async Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(
        string text,
        string sessionToken,
        double lat,
        double lng,
        double radius,
        string language,
        string locationType,
        CancellationToken cancellationToken)
    {
        if (_redis is null)
        {
            return null;
        }

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_maxWaitTime);

            var placeKey = $"{AutoCompleteKeyPrefix}:{language}:{text.ToLower()}";
            var value = await _redis.StringGetAsync(placeKey).WaitAsync(cts.Token);

            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<IEnumerable<AutoComplete>>(value.ToString());
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Redis operation cancelled");
        }
        catch (Exception e)
        {
            logger.LogWarning("Redis operation error {Message} during {Operation}", e.Message,
                nameof(GetAutoCompleteAsync));
        }

        return null;
    }

    public async Task<bool> SetAutoCompleteAsync(
        IEnumerable<AutoComplete> autoCompletes,
        string text,
        string language,
        CancellationToken cancellationToken)
    {
        if (_redis is null)
        {
            return false;
        }

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_maxWaitTime);

            var random = Random.Shared;
            var jitter = TimeSpan.FromMinutes(random.Next(0, AutoCompleteJitterMaxMins));
            var ttl = _autoCompleteDefaultTtl + jitter;

            var autoCompleteKey = $"{AutoCompleteKeyPrefix}:{language}:{text.ToLower()}";
            var autoCompleteJson = JsonSerializer.Serialize(autoCompletes);
            return await _redis.StringSetAsync(autoCompleteKey, autoCompleteJson, ttl).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Redis operation cancelled");
        }
        catch (Exception e)
        {
            logger.LogWarning("Redis operation error {Message} during {Operation}", e.Message,
                nameof(SetAutoCompleteAsync));
        }

        return false;
    }
}