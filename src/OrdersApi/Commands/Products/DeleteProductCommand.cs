using MediatR;

namespace OrdersApi.Commands.Products;

public record DeleteProductCommand(Guid ProductId) : IRequest<bool>;
