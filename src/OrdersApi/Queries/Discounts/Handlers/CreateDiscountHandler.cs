using MediatR;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Discounts.Handlers;

public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand, Discount>
{
    private readonly IDiscountRepository _repository;

    public CreateDiscountHandler(IDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Discount> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = new Discount
        {
            OrderType    = request.OrderType,
            DiscountType = request.DiscountType,
            Rate         = request.Rate,
            Active       = true
        };

        return await _repository.CreateAsync(discount);
    }
}
