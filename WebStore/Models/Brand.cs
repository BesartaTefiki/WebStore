namespace WebStore.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; 

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
