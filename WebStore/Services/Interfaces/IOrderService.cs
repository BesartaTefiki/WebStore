using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.DTOs;
using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);

        Task<Order> CreateAsync(int clientId, List<OrderItemDto> items);

        Task UpdateStatusAsync(int orderId, string status);
    }
}
