namespace WebStore.Models
{
    public class Discount
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
