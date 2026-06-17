using MediatR;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Commands.Products.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name        = request.Name,
            ImageURL    = request.ImageURL,
            Description = request.Description,
            Price       = request.Price
        };

        return await _repository.CreateAsync(product);
    }
}
