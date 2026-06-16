using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;

namespace OrdersApi.Tests.Domain;

public class OrderTests
{
    [Theory]
    [InlineData(OrderStatus.New, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Completed)]
    public void CanTransitionTo_ShouldReturnTrue_ForValidTransitions(OrderStatus from, OrderStatus to)
    {
        var order = new Order { Status = from };
        Assert.True(order.CanTransitionTo(to));
    }

    [Theory]
    [InlineData(OrderStatus.New, OrderStatus.Shipped)]
    [InlineData(OrderStatus.New, OrderStatus.Completed)]
    [InlineData(OrderStatus.New, OrderStatus.New)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.New)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Completed)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped, OrderStatus.New)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Completed, OrderStatus.New)]
    [InlineData(OrderStatus.Completed, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Completed, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Completed, OrderStatus.Completed)]
    public void CanTransitionTo_ShouldReturnFalse_ForInvalidTransitions(OrderStatus from, OrderStatus to)
    {
        var order = new Order { Status = from };
        Assert.False(order.CanTransitionTo(to));
    }
}
