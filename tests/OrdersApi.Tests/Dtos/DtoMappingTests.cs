using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Dtos;

namespace OrdersApi.Tests.Dtos;

public class DtoMappingTests
{
    [Fact]
    public void OrderItemResponse_From_ShouldMapAllFields()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Sneaker" };
        var item = new OrderItem
        {
            ProductId = product.Id,
            Product   = product,
            UnitPrice = 99m,
            Quantity  = 2
        };

        var dto = OrderItemResponse.From(item);

        Assert.Equal(item.ProductId, dto.ProductId);
        Assert.Equal("Sneaker",     dto.ProductName);
        Assert.Equal(99m,           dto.UnitPrice);
        Assert.Equal(2,             dto.Quantity);
    }

    [Fact]
    public void OrderResponse_From_ShouldMapAllFields_IncludingItems()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Cap" };
        var item = new OrderItem { ProductId = product.Id, Product = product, UnitPrice = 30m, Quantity = 1 };
        var order = new Order
        {
            Id               = Guid.NewGuid(),
            CreationDateTime = DateTime.UtcNow,
            Type             = OrderType.Express,
            OriginalValue    = 30m,
            DebitedValue     = 34.5m,
            User             = "user@test.com",
            Status           = OrderStatus.Confirmed,
            TrackingURL      = "https://tracking.com",
            Items            = [item]
        };

        var dto = OrderResponse.From(order);

        Assert.Equal(order.Id,            dto.Id);
        Assert.Equal("Express",           dto.Type);
        Assert.Equal(30m,                 dto.OriginalValue);
        Assert.Equal(34.5m,               dto.DebitedValue);
        Assert.Equal("user@test.com",     dto.User);
        Assert.Equal("Confirmed",         dto.Status);
        Assert.Equal("https://tracking.com", dto.TrackingURL);
        Assert.Single(dto.Items);
        Assert.Equal("Cap",               dto.Items[0].ProductName);
    }

    [Fact]
    public void OrderResponse_ToString_ShouldContainMappedFields()
    {
        var order = new Order
        {
            Id            = Guid.NewGuid(),
            Type          = OrderType.Standard,
            Status        = OrderStatus.New,
            User          = "user@test.com",
            TrackingURL   = "",
            OriginalValue = 0m,
            DebitedValue  = 0m
        };

        var str = OrderResponse.From(order).ToString();

        Assert.Contains("user@test.com", str);
        Assert.Contains("Standard",      str);
    }

    [Fact]
    public void ProductResponse_From_ShouldMapAllFields()
    {
        var product = new Product
        {
            Id          = Guid.NewGuid(),
            Name        = "Sneaker",
            ImageURL    = "https://img.com/sneaker.jpg",
            Description = "Running shoe",
            Price       = 149.99m
        };

        var dto = ProductResponse.From(product);

        Assert.Equal(product.Id,          dto.Id);
        Assert.Equal("Sneaker",           dto.Name);
        Assert.Equal("https://img.com/sneaker.jpg", dto.ImageURL);
        Assert.Equal("Running shoe",      dto.Description);
        Assert.Equal(149.99m,             dto.Price);
    }

    [Fact]
    public void DiscountResponse_From_ShouldMapAllFields()
    {
        var discount = new Discount
        {
            Id           = Guid.NewGuid(),
            OrderType    = OrderType.Express,
            DiscountType = DiscountType.Value,
            Rate         = 10m,
            Active       = true
        };

        var dto = DiscountResponse.From(discount);

        Assert.Equal(discount.Id, dto.Id);
        Assert.Equal("Express",   dto.OrderType);
        Assert.Equal("Value",     dto.DiscountType);
        Assert.Equal(10m,         dto.Rate);
        Assert.True(dto.Active);
    }
}
