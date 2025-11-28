using System.ComponentModel.DataAnnotations;

namespace WebStore.DTOs
{
    public class DiscountRequestDto
    {
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal DiscountPercent { get; set; }
    }
}
