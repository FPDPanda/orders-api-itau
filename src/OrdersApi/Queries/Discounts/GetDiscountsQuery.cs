using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Discounts;

public record GetDiscountsQuery : IRequest<IReadOnlyList<Discount>>;
