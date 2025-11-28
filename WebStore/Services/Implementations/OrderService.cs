using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IClientRepository clientRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllWithDetailsAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Order> CreateAsync(int clientId, List<OrderItemDto> items)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException(
                    $"Client with id {clientId} does not exist.");
            }

            if (items == null || items.Count == 0)
            {
                throw new InvalidOperationException("Order must contain at least one item.");
            }

            var order = new Order
            {
                ClientId = clientId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            foreach (var itemDto in items)
            {
                var product = await _productRepository.GetProductById(itemDto.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException(
                        $"Product with id {itemDto.ProductId} does not exist.");
                }

                var reserved = await _orderRepository
                    .GetReservedQuantityForProductAsync(itemDto.ProductId);

                var available = product.Quantity - reserved;

                if (available <= 0)
                {
                    throw new InvalidOperationException(
                        $"Product '{product.Name}' is out of stock. Initial stock: {product.Quantity}, already reserved: {reserved}.");
                }

                if (itemDto.Quantity > available)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product '{product.Name}'. Available: {available}, requested: {itemDto.Quantity}.");
                }

                order.Items.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity
                });
            }

            await _orderRepository.AddAsync(order);
            return order;
        }

        public async Task UpdateStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException(
                    $"Order with id {orderId} does not exist.");
            }

            order.Status = status;
            await _orderRepository.UpdateAsync(order);
        }
    }
}
