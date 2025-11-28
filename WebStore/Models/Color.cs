using System.Text.Json.Serialization;

namespace WebStore.Models
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
