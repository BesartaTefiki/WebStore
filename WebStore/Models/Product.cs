using System.Text.Json.Serialization;

namespace WebStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPercent { get; set; }
        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int GenderId { get; set; }
        public Gender? Gender { get; set; }
        public ICollection<Size> Sizes { get; set; } = new List<Size>();
        public ICollection<Color> Colors { get; set; } = new List<Color>();

        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
