using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Products;

public record GetProductByIdQuery(Guid ProductId) : IRequest<Product?>;
