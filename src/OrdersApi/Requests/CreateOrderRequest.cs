using System.ComponentModel.DataAnnotations;
using OrdersApi.Domain.Enums;

namespace OrdersApi.Requests;

public record CreateOrderRequest(
    OrderType Type,
    [Required]
    [MinLength(1, ErrorMessage = "At least one product is required.")]
    IReadOnlyList<Guid> ProductIds,
    [Required]
    string User,
    string TrackingURL);
