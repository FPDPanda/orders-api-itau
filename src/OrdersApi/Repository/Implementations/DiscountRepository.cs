using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Repository.Implementations;

public class DiscountRepository : IDiscountRepository
{
    private readonly OrdersDbContext _context;

    public DiscountRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Discount>> GetAllActiveAsync()
    {
        return await _context.Discounts
            .Where(d => d.Active)
            .ToListAsync();
    }

    public async Task<Discount?> GetActiveByOrderTypeAsync(OrderType orderType)
    {
        return await _context.Discounts
            .FirstOrDefaultAsync(d => d.OrderType == orderType && d.Active);
    }

    public async Task<Discount> CreateAsync(Discount discount)
    {
        discount.Id = Guid.NewGuid();
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount is null)
            return false;

        discount.Active = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
