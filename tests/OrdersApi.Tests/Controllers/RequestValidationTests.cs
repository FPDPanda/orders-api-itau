using System.ComponentModel.DataAnnotations;
using Xunit;
using OrdersApi.Controllers;
using OrdersApi.Domain.Enums;

namespace OrdersApi.Tests.Controllers;

public class RequestValidationTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void CreateOrderRequest_ShouldPassValidation_WhenValid()
    {
        var request = new CreateOrderRequest(OrderType.Standard, [Guid.NewGuid()], "user@test.com", "");
        Assert.Empty(Validate(request));
    }

    [Fact]
    public void CreateOrderRequest_ShouldFailValidation_WhenProductIdsIsEmpty()
    {
        var request = new CreateOrderRequest(OrderType.Standard, [], "user@test.com", "");
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void CreateOrderRequest_ShouldFailValidation_WhenUserIsEmpty()
    {
        var request = new CreateOrderRequest(OrderType.Standard, [Guid.NewGuid()], "", "");
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void CreateProductRequest_ShouldPassValidation_WhenValid()
    {
        var request = new CreateProductRequest("Sneaker", "https://img.com/sneaker.jpg", "A nice sneaker", 99.99m);
        Assert.Empty(Validate(request));
    }

    [Fact]
    public void CreateProductRequest_ShouldFailValidation_WhenNameIsEmpty()
    {
        var request = new CreateProductRequest("", "https://img.com", "desc", 10m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void CreateProductRequest_ShouldFailValidation_WhenImageURLIsEmpty()
    {
        var request = new CreateProductRequest("Sneaker", "", "desc", 10m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void CreateProductRequest_ShouldFailValidation_WhenDescriptionIsEmpty()
    {
        var request = new CreateProductRequest("Sneaker", "https://img.com", "", 10m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void CreateProductRequest_ShouldFailValidation_WhenPriceIsZero()
    {
        var request = new CreateProductRequest("Sneaker", "https://img.com", "desc", 0m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void CreateProductRequest_ShouldFailValidation_WhenPriceIsNegative()
    {
        var request = new CreateProductRequest("Sneaker", "https://img.com", "desc", -1m);
        Assert.NotEmpty(Validate(request));
    }
}
