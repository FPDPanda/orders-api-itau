using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Requests;

namespace OrdersApi.Tests.Controllers;

public class RequestValidationTests
{
    // Validates against constructor parameter attributes — the same place ASP.NET Core
    // model binding reads validation metadata for record primary constructors.
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var ctor = model.GetType().GetConstructors().First();

        foreach (var param in ctor.GetParameters())
        {
            var prop = model.GetType().GetProperty(param.Name!,
                BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) continue;

            var value = prop.GetValue(model);
            foreach (var attr in param.GetCustomAttributes<ValidationAttribute>())
            {
                var ctx = new ValidationContext(model) { MemberName = param.Name };
                if (!attr.IsValid(value))
                    results.Add(new ValidationResult(attr.FormatErrorMessage(param.Name!)));
            }
        }

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

    [Fact]
    public void UpdateProductRequest_ShouldPassValidation_WhenValid()
    {
        var request = new UpdateProductRequest("Sneaker", "https://img.com/sneaker.jpg", "A nice sneaker", 99.99m);
        Assert.Empty(Validate(request));
    }

    [Fact]
    public void UpdateProductRequest_ShouldFailValidation_WhenNameIsEmpty()
    {
        var request = new UpdateProductRequest("", "https://img.com", "desc", 10m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void UpdateProductRequest_ShouldFailValidation_WhenImageURLIsEmpty()
    {
        var request = new UpdateProductRequest("Sneaker", "", "desc", 10m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void UpdateProductRequest_ShouldFailValidation_WhenDescriptionIsEmpty()
    {
        var request = new UpdateProductRequest("Sneaker", "https://img.com", "", 10m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void UpdateProductRequest_ShouldFailValidation_WhenPriceIsZero()
    {
        var request = new UpdateProductRequest("Sneaker", "https://img.com", "desc", 0m);
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void UpdateProductRequest_ShouldFailValidation_WhenPriceIsNegative()
    {
        var request = new UpdateProductRequest("Sneaker", "https://img.com", "desc", -1m);
        Assert.NotEmpty(Validate(request));
    }
}
