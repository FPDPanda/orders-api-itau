using Xunit;
using OrdersApi.Domain.Models;

namespace OrdersApi.Tests.Domain;

public class OrderItemTests
{
    [Fact]
    public void OrderItem_ShouldSetAndGetAllProperties()
    {
        var order   = new Order   { Id = Guid.NewGuid() };
        var product = new Product { Id = Guid.NewGuid(), Name = "Sneaker", Price = 99m };
        var itemId  = Guid.NewGuid();

        var item = new OrderItem
        {
            Id        = itemId,
            OrderId   = order.Id,
            ProductId = product.Id,
            Quantity  = 3,
            Order     = order,
            Product   = product
        };

        Assert.Equal(itemId,     item.Id);
        Assert.Equal(order.Id,   item.OrderId);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(3,          item.Quantity);
        Assert.Equal(order,      item.Order);
        Assert.Equal(product,    item.Product);
    }
}
