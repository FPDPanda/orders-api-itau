using MediatR;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;

namespace OrdersApi.Commands.Discounts;

public record CreateDiscountCommand(
    OrderType OrderType,
    DiscountType DiscountType,
    decimal Rate) : IRequest<Discount>;
