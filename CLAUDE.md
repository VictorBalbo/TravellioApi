# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run (development)
dotnet run

# Publish
dotnet publish -c Release

# Docker
docker build -t travellio-api .
docker run -p 8080:8080 travellio-api
```

## Configuration

Required keys in `appsettings.Development.json` (or environment variables):

| Key | Description                                                                             |
|-----|-----------------------------------------------------------------------------------------|
| `SqlConnectionString` | PostgreSQL (Npgsql) connection string — snake_case naming is applied automatically      |
| `RedisConnectionString` | StackExchange.Redis format (not `rediss://` URI) — omitting disables caching            |
| `WanderlogPlaceDetailsUrl` | Wanderlog API endpoit for fetching Place details. Must contain `{placeId}` placeholder  |
| `WanderlogMetadataUrl` | Wanderlog API endpoit for fetching Place metadata. Must contain `{placeId}` placeholder |

Redis connection string format: `host:port,password=xxx,ssl=true` — StackExchange.Redis does **not** parse `rediss://` URIs.

## Architecture

### Data model hierarchy

```
Trip
├── Destination[]   (PlaceId → Place via PlaceService)
│   ├── Accommodation[]  (PlaceId → Place)
│   └── Activity[]       (PlaceId → Place)
└── Transportation[]
    ├── Leg[]  (DeparturePlaceId + ArrivalPlaceId → Place)
    ├── ArrivalDestinationId → Destination (FK, optional)
    └── DepartureDestinationId → Destination (FK, optional)
```

`Place` is a DTO — never persisted to SQL, populated at query time by `PlaceService`.

### Place enrichment

`PlaceService` implements cache-aside:
1. `InternalProvider` — Redis, silently degrades on failure, 3-second timeout per operation. Cache key: `place:{placeId}`. TTL: 7 days + random jitter of 0–7 days.
2. `WanderlogProvider` — fires two HTTP calls in parallel (details + metadata endpoints) and merges results into a `Place`.

Enrichment depth depends on the endpoint:
- `GET /api/trips` — no enrichment, no `Include`s
- `GET /api/trips/{id}` — enriches `Destination.Place` and `Leg` terminals (`DeparturePlace`/`ArrivalPlace`); includes `Destinations.Activities` via EF but does **not** enrich them
- `GET /api/trips/{tripId}/destinations/{id}` — enriches the destination and all its accommodations and activities
- `GET /api/trips/{tripId}/transportations/{id}` — enriches `Transportation.Arrival.Place` and `Transportation.Departure.Place` (the linked `Destination` entities)

All place fetches within a single request are fanned out via `Task.WhenAll`.

### Repository pattern

`BaseRepository<T>` (where `T : IModel`) provides `GetByIdAsync`, `AddOrUpdateAsync`, and `DeleteAsync`. Concrete repositories override `GetByIdAsync` to add `Include`/`ThenInclude` chains and call enrichment helpers.

### EF configuration

Each entity has a dedicated `IEntityTypeConfiguration<T>` in `DbContexts/`. All FK columns, cascade deletes, and string lengths are defined there — not via data annotations. `Constants.PlaceIdSize = 40` is the standard max length for `PlaceId` columns.

### API routes

- `GET /api/trips` — all trips (no enrichment)
- `GET /api/trips/{id}` — trip with destinations + transportations (partial enrichment)
- `POST /api/trips` — upsert trip
- `DELETE /api/trips/{id}` — delete trip
- `GET /api/trips/{tripId}/destinations` — all destinations for a trip (no enrichment)
- `GET /api/trips/{tripId}/destinations/{id}` — destination with accommodations + activities (full enrichment)
- `POST /api/trips/{tripId}/destinations` — upsert destination
- `DELETE /api/trips/{tripId}/destinations/{id}` — delete destination
- `GET /api/trips/{tripId}/transportations` — all transportations for a trip (no enrichment)
- `GET /api/trips/{tripId}/transportations/{id}` — transportation with legs + linked destination enrichment
- `POST /api/trips/{tripId}/transportations` — upsert transportation
- `DELETE /api/trips/{tripId}/transportations/{id}` — delete transportation

OpenAPI is only mapped in Development (`app.MapOpenApi()`). Auth is not yet implemented (marked TODO in controllers).
