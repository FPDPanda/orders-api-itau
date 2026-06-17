using OrdersApi.Domain.Models;

namespace OrdersApi.Dtos;

public record DiscountResponse(
    Guid Id,
    string OrderType,
    string DiscountType,
    decimal Rate,
    bool Active)
{
    public static DiscountResponse From(Discount discount) => new(
        discount.Id,
        discount.OrderType.ToString(),
        discount.DiscountType.ToString(),
        discount.Rate,
        discount.Active);
}
