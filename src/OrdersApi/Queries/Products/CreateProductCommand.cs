using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Products;

public record CreateProductCommand(
    string ImageURL,
    string Description,
    decimal Price) : IRequest<Product>;
