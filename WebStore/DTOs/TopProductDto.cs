namespace WebStore.Models
{
    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int QuantitySold { get; set; }
        public decimal TotalEarnings { get; set; }
    }
}
