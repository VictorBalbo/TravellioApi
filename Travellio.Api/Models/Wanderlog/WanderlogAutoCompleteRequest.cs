namespace Travellio.Api.Models.Wanderlog;

public class WanderlogAutoCompleteRequest
{
    public required string Input { get; set; }
    public required string SessionToken { get; set; }
    public WanderlogAutoCompleteLocation? Location { get; set; }
    public double? Radius { get; set; }
    public string? Language { get; set; }
}

public class WanderlogAutoCompleteLocation
{
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}