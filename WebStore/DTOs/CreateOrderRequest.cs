using System.Collections.Generic;

namespace WebStore.DTOs
{
    public class CreateOrderRequest
    {
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
