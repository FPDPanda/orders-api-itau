using OrdersApi.Domain.Enums;

namespace OrdersApi.Requests;

public record CreateDiscountRequest(OrderType OrderType, DiscountType DiscountType, decimal Rate);
