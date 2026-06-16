using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Products;

public record UpdateProductCommand(
    Guid ProductId,
    string ImageURL,
    string Description,
    decimal Price) : IRequest<Product?>;
