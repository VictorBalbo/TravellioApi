namespace TravellioApi.Models.Wanderlog;

public record WanderlogResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
}