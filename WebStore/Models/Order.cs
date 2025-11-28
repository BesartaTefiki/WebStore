using System.Text.Json.Serialization;

namespace WebStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public string Status { get; set; } = "Pending";           

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int? UserId { get; set; } 
        public User? User { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
