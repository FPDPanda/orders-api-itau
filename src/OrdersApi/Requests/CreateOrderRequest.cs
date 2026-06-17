using System.ComponentModel.DataAnnotations;
using OrdersApi.Domain.Enums;

namespace OrdersApi.Requests;

public record CreateOrderRequest(
    OrderType Type,
    [property: Required]
    [property: MinLength(1, ErrorMessage = "At least one product is required.")]
    IReadOnlyList<Guid> ProductIds,
    [property: Required]
    string User,
    string TrackingURL);
