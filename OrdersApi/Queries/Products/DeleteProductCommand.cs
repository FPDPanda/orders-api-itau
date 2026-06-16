using MediatR;

namespace OrdersApi.Queries.Products;

public record DeleteProductCommand(Guid ProductId) : IRequest<bool>;
