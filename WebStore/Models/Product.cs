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

        public int SizeId { get; set; }
        public Size? Size { get; set; }

        public int ColorId { get; set; }
        public Color? Color { get; set; }

        public int GenderId { get; set; }
        public Gender? Gender { get; set; }

        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
