namespace Travellio.Models.Wanderlog
{
    public class WanderlogResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
    }
}
