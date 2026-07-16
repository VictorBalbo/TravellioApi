# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Solution structure

```
TravellioApi.slnx
├── Travellio.Domain           — entities, DTOs, constants (no framework deps)
├── Travellio.Infrastructure   — EF DbContext, EF configs, Redis, Serilog setup
├── Travellio.Api              — main ASP.NET Core web API (net10.0)
├── Travellio.Airports         — standalone utility app: imports airports from CSV into the DB
├── Travellio.UnitTests        — xUnit unit tests for service logic (NSubstitute mocks)
└── Travellio.IntegrationTests — xUnit integration tests with real PostgreSQL (Testcontainers)
```

Only `Travellio.Api` and `Travellio.Airports` have Dockerfiles.

## Commands

```bash
# Build
dotnet build

# Run main API (from Travellio.Api/)
dotnet run --project Travellio.Api

# Run tests
dotnet test

# Publish
dotnet publish -c Release

# Docker (main API)
docker build -f Travellio.Api/Dockerfile -t travellio-api .
docker run -p 8080:8080 travellio-api

# Docker (airports importer)
docker build -f Travellio.Airports/Dockerfile -t travellio-airports .
docker run -p 8080:8080 travellio-airports
```

## Configuration

Connection strings and external URLs go in `appsettings.Development.json` (or environment variables):

| Key | Description |
|-----|-------------|
| `ConnectionStrings:Sql` | PostgreSQL (Npgsql) connection string — snake_case naming applied automatically |
| `ConnectionStrings:Redis` | StackExchange.Redis format — omitting key disables caching (falls back to NoCacheProvider) |
| `WanderlogPlaceDetailsUrl` | Wanderlog details endpoint; must contain `{placeId}` placeholder |
| `WanderlogMetadataUrl` | Wanderlog metadata endpoint; must contain `{placeId}` placeholder |
| `WanderlogAutoCompleteUrl` | Wanderlog autocomplete endpoint; request JSON is appended as `?request=` query param |
| `R2:AccountId` | Cloudflare account ID; used to build the R2 S3-compatible endpoint (`https://{accountId}.r2.cloudflarestorage.com`) — **required**, app fails to start if missing |
| `R2:AccessKeyId` / `R2:SecretAccessKey` | R2 API token credentials (S3 auth) — **required** |
| `R2:BucketName` | R2 bucket that image uploads are written to — **required** |
| `R2:PublicUrl` | Base URL used to build the public link returned for an uploaded image (custom domain or public `r2.dev` bucket URL) — **required** |

Redis connection string format: `host:port,password=xxx,ssl=true` — StackExchange.Redis does **not** parse `rediss://` URIs.

Serilog is configured via `appsettings.json` (`ReadFrom.Configuration`). Seq sink is included (default: `http://localhost:5341`).

## Architecture

### Data model hierarchy

```
Trip
├── Destination[]   (PlaceId → Place via PlaceService)
│   ├── Accommodation[]  (PlaceId → Place)
│   └── Activity[]       (PlaceId → Place)
└── Transportation[]
    ├── Leg[]  (DeparturePlaceId + ArrivalPlaceId → Place)
    │           Leg also stores ShortName, Description, Coordinates directly in DB
    ├── ArrivalDestinationId → Destination (FK, optional)
    └── DepartureDestinationId → Destination (FK, optional)
```

`Place` is a DTO — never persisted to SQL, populated at query time by `PlaceService`.

`Airport` is a standalone entity (IataCode as primary key, no GuidV7) used only by the Airports importer.

### Query / Repository split

**Queries** (read path) — `Travellio.Api/Queries/` — project directly to DTOs using EF LINQ `Select` + `AsSplitQuery`. No tracked entities, no `Include`/`ThenInclude`. Each query class calls `PlaceService` for enrichment after fetching.

**Repositories** (write path) — `Travellio.Api/Repositories/` — `BaseRepository<T>` (where `T : IBaseEntity`) provides `GetByIdAsync`, `AddOrUpdateAsync`, and `DeleteAsync`. Concrete repositories override `GetByIdAsync` with `Include`/`ThenInclude` chains for write-side needs (cascade upserts). Controllers call Services → Repositories for mutations.

### Place enrichment

`PlaceService` implements cache-aside for both place details and autocomplete:

1. `CachedProvider` (Redis, `ICachedPlaceProvider`):
   - Place details: key `place:{placeId}`, TTL 7 days + 0–7 days jitter. 3-second operation timeout; silently degrades on failure.
   - Autocomplete: key `autocomplete:{language}:{text.ToLower()}`, TTL 30 min + 0–30 min jitter.
   - Falls back to `NoCacheProvider` (no-op) when `ConnectionStrings:Redis` is absent.
2. `WanderlogProvider` (external, `IPlaceProvider`):
   - Place details: fires details + metadata HTTP calls in parallel via `Task.WhenAll`, merges into `Place`.
   - Autocomplete: serializes request to JSON, passes as `?request=` query param.

All place fetches within a single request are fanned out via `Task.WhenAll`.

`PlaceService` sets `IDiagnosticContext["CacheResult"] = "Hit"/"Miss"` for Serilog request logging.

Enrichment depth per endpoint:

| Endpoint | Enrichment |
|----------|------------|
| `GET /Api/Trips` | None |
| `GET /Api/Trips/{id}` | `Destination.Place` + `Leg.DeparturePlace` + `Leg.ArrivalPlace` for all legs |
| `GET /Api/Trips/{tripId}/Destinations` | None |
| `GET /Api/Trips/{tripId}/Destinations/{id}` | Destination + all Accommodations + all Activities |
| `GET /Api/Trips/{tripId}/Transportations` | None |
| `GET /Api/Trips/{tripId}/Transportations/{id}` | Leg terminals + `Arrival.Place` + `Departure.Place` |
| `GET /Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}` | Activity.Place |
| `GET /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}` | Accommodation.Place |
| `GET /Api/Places/{id}` | Fetches and caches single place |

Single-resource GET endpoints accept an `?enrichPlaces=true` (default) query parameter to skip enrichment when not needed.

### EF configuration

Each entity has a dedicated `IEntityTypeConfiguration<T>` in `Travellio.Infrastructure/DbContexts/`. FK columns, cascade deletes, and string lengths are defined there — not via data annotations. `Constants.PlaceIdSize = 40` is the standard max length for `PlaceId` columns. `TransportationType` is stored as a string. `Price` and `Coordinates` are owned entities. All entities except `Airport` use GuidV7 primary keys (auto-generated via `GuidV7ValueGenerator`).

`AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` is set in `AddDatabaseInfrastructure` to handle timestamp columns.

### Logging

Serilog with `UseSerilogRequestLogging`. Each HTTP request log is enriched with `RouteTemplate` (raw route pattern) via `AddRequestLogging()` middleware. `CacheResult` (`Hit` / `Miss`) is set by `PlaceService` via `IDiagnosticContext`.

### DependencyInjection extension blocks

`Travellio.Infrastructure/DependencyInjection.cs` uses C# 13 `extension` blocks (preview feature) to add methods directly onto `IServiceCollection`, `IHostBuilder`, and `WebApplication`:
- `services.AddDatabaseInfrastructure(configuration)` — registers AppDbContext
- `services.AddRedisInfrastructure(configuration)` — registers `IConnectionMultiplexer` (skipped if key absent)
- `host.AddSerilog()` — configures Serilog from `appsettings.json`
- `app.AddRequestLogging()` — wires up `UseSerilogRequestLogging` middleware

## API routes

**Trips**
- `GET    /Api/Trips` — all trips (flat, no enrichment)
- `GET    /Api/Trips/{tripId}` — trip with destinations + transportations + legs (enriched)
- `POST   /Api/Trips` — upsert trip
- `DELETE /Api/Trips/{tripId}` — delete trip (cascades to destinations + transportations)

**Destinations**
- `GET    /Api/Trips/{tripId}/Destinations` — all destinations for a trip (no enrichment)
- `GET    /Api/Trips/{tripId}/Destinations/{id}` — destination with accommodations + activities (enriched)
- `POST   /Api/Trips/{tripId}/Destinations` — upsert destination
- `DELETE /Api/Trips/{tripId}/Destinations/{id}` — delete destination (cascades to activities + accommodations)

**Transportations**
- `GET    /Api/Trips/{tripId}/Transportations` — all transportations with legs (no enrichment)
- `GET    /Api/Trips/{tripId}/Transportations/{id}` — transportation with legs + linked destination enrichment
- `POST   /Api/Trips/{tripId}/Transportations` — upsert transportation
- `DELETE /Api/Trips/{tripId}/Transportations/{id}` — delete transportation

**Activities**
- `GET    /Api/Trips/{tripId}/Destinations/{destinationId}/Activities` — all activities (no enrichment)
- `GET    /Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}` — activity with place enrichment
- `POST   /Api/Trips/{tripId}/Destinations/{destinationId}/Activities` — upsert activity
- `DELETE /Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}` — delete activity

**Accommodations**
- `GET    /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations` — all accommodations (no enrichment)
- `GET    /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}` — accommodation with place enrichment
- `POST   /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations` — upsert accommodation
- `DELETE /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}` — delete accommodation

**Places**
- `GET /Api/Places/{id}` — fetch and cache a single place from Wanderlog
- `GET /Api/Places/Autocomplete?text=&lat=&lng=&radius=&language=&sessionToken=` — place autocomplete (sessionToken optional)

**Images**
- `POST /Api/Images` — upload an image (multipart/form-data, field `file`) to the Cloudflare R2 bucket; returns `{ url }`. Max 10MB; allowed types: jpeg, png, webp, gif

OpenAPI is only mapped in Development (`app.MapOpenApi()`). Auth is not yet implemented (marked TODO in controllers).

## Travellio.Airports utility

Standalone web app that seeds the `airports` table from a CSV file (OurAirports format). Exposes a single endpoint:

```
POST /import?filePath=/path/to/airports.csv
```

`AirportImporter` reads the CSV with CsvHelper (lenient config — missing/bad fields are ignored), skips rows with no IATA code, and bulk-inserts only rows not already present in the DB (keyed by `IataCode`, case-insensitive check).

## Tests

### Travellio.UnitTests

xUnit + NSubstitute. Tests `PlaceService` in isolation by mocking `IPlaceProvider`, `ICachedPlaceProvider`, and `IDiagnosticContext`. Covers:
- Cache hit: returns cached place, does not call external provider or write to cache
- Cache miss + valid external result: stores in cache, returns place
- Cache miss + place with empty name: returns `null` (name is required)
- Cache miss + external returns `null`: returns `null`
- AutoComplete: parallel cache hit/miss and filtering scenarios

### Travellio.IntegrationTests

xUnit + `WebApplicationFactory<Program>` + Testcontainers (postgres:18). `ApiFactory`:
- Spins up a real PostgreSQL container per test collection
- Creates schema via `EnsureCreatedAsync`
- Replaces `IPlaceProvider` with `FakeWanderlogProvider` (returns `"Paris, France"` for any placeId)
- Overrides `AppDbContext` connection string to point at the container

Test classes (`TripTests`, `DestinationTests`, `ActivityTests`, `AccommodationTests`) use `IClassFixture<ApiFactory>` and `IAsyncLifetime` for per-test DB cleanup. Each test class has helper seed methods to set up prerequisite data.

Covered scenarios per resource: 404 on empty, 200 with list, 404 not found, 200 with data, place enrichment, POST returns 201 with Location header, POST persists to DB, POST updates existing (upsert), DELETE 404, DELETE 204, DELETE removes from DB, cascade delete.