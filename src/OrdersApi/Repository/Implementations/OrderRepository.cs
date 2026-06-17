using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.Id = Guid.NewGuid();
        order.CreationDateTime = DateTime.UtcNow;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> AddItemAsync(Guid orderId, Guid productId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return null;

        var product = await _context.Products.FindAsync(productId);
        if (product is null) return null;

        var existing = order.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity++;
        }
        else
        {
            order.Items.Add(new OrderItem
            {
                ProductId = productId,
                Product   = product,
                Quantity  = 1,
                UnitPrice = product.Price
            });
        }

        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> RemoveItemAsync(Guid orderId, Guid productId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return false;

        var item = order.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return false;

        order.Items.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Order?> UpdateStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return null;

        order.Status = status;
        await _context.SaveChangesAsync();
        return order;
    }
}
