using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Products;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string ImageURL,
    string Description,
    decimal Price) : IRequest<Product?>;
