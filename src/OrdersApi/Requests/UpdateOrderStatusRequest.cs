using OrdersApi.Domain.Enums;

namespace OrdersApi.Requests;

public record UpdateOrderStatusRequest(OrderStatus Status);
