namespace WebStore.DTOs
{
    public class ProductQuantityDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public int InitialQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public int CurrentQuantity { get; set; }
    }
}
