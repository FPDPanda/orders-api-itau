using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;

namespace OrdersApi.Repository.Interfaces;

public interface IDiscountRepository
{
    Task<IReadOnlyList<Discount>> GetAllActiveAsync();
    Task<Discount?> GetActiveByOrderTypeAsync(OrderType orderType);
    Task<Discount> CreateAsync(Discount discount);
    Task<bool> DeactivateAsync(Guid id);
}
