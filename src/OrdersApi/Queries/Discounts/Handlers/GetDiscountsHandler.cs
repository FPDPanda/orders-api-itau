using MediatR;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Discounts.Handlers;

public class GetDiscountsHandler : IRequestHandler<GetDiscountsQuery, IReadOnlyList<Discount>>
{
    private readonly IDiscountRepository _repository;

    public GetDiscountsHandler(IDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Discount>> Handle(GetDiscountsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllActiveAsync();
    }
}
