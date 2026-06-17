using MediatR;

namespace OrdersApi.Commands.Discounts;

public record DeactivateDiscountCommand(Guid Id) : IRequest<bool>;
