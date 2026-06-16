using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Repository.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly OrdersDbContext _context;

    public ProductRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        product.Id = Guid.NewGuid();
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _context.Products.FindAsync(productId);
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds)
    {
        return await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<Product?> UpdateAsync(Guid productId, Product product)
    {
        var existing = await _context.Products.FindAsync(productId);
        if (existing is null)
            return null;

        existing.ImageURL = product.ImageURL;
        existing.Description = product.Description;
        existing.Price = product.Price;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
