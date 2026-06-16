using OrdersApi.Domain.Models;

namespace OrdersApi.Repository.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(Guid productId);
    Task<Product?> UpdateAsync(Guid productId, Product product);
    Task<bool> DeleteAsync(Guid productId);
}
