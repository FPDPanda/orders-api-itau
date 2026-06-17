using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Commands.Products;

public record CreateProductCommand(
    string Name,
    string ImageURL,
    string Description,
    decimal Price) : IRequest<Product>;
