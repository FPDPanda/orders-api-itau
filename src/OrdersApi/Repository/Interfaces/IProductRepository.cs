using OrdersApi.Domain.Models;

namespace OrdersApi.Repository.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(Guid productId);
    Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds);
    Task<Product?> UpdateAsync(Guid productId, Product product);
    Task<bool> DeleteAsync(Guid productId);
}
