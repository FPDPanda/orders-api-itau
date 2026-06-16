using OrdersApi.Domain.Models;

namespace OrdersApi.Repository.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<Order?> AddItemAsync(Guid orderId, Guid itemId);
    Task<bool> RemoveItemAsync(Guid orderId, Guid itemId);
}
