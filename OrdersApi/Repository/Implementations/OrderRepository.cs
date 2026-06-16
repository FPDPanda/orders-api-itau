using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
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
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> AddItemAsync(Guid orderId, Guid itemId)
    {
        var order = await _context.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null)
            return null;

        var product = await _context.Products.FindAsync(itemId);
        if (product is null)
            return null;

        order.Products.Add(product);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<bool> RemoveItemAsync(Guid orderId, Guid itemId)
    {
        var order = await _context.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return false;

        var product = order.Products.FirstOrDefault(p => p.Id == itemId);
        if (product is null)
            return false;

        order.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
