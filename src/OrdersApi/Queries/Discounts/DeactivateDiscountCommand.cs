using MediatR;

namespace OrdersApi.Queries.Discounts;

public record DeactivateDiscountCommand(Guid Id) : IRequest<bool>;
