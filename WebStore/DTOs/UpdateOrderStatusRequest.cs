namespace WebStore.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = null!; // Pending, Confirmed, Cancelled...
    }
}
