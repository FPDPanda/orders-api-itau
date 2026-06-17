using MediatR;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Commands.Discounts.Handlers;

public class DeactivateDiscountHandler : IRequestHandler<DeactivateDiscountCommand, bool>
{
    private readonly IDiscountRepository _repository;

    public DeactivateDiscountHandler(IDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeactivateDiscountCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeactivateAsync(request.Id);
    }
}
