using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;

namespace OrdersApi.Tests.Domain;

public class DiscountTests
{
    [Theory]
    [InlineData( 0.15,  100, 115.00)]  // +15% surcharge
    [InlineData(-0.10,  100,  90.00)]  // -10% discount
    [InlineData( 0.00,  100, 100.00)]  // no change
    public void Apply_Percentage_ShouldCalculateCorrectly(decimal rate, decimal original, decimal expected)
    {
        var discount = new Discount { DiscountType = DiscountType.Percentage, Rate = rate };
        Assert.Equal(expected, discount.Apply(original));
    }

    [Theory]
    [InlineData( 10.00, 100,  110.00)]  // +$10 surcharge
    [InlineData(-10.00, 100,   90.00)]  // -$10 discount
    [InlineData(-200.00, 100,   0.00)]  // discount exceeds price → clamped to 0
    public void Apply_Value_ShouldCalculateCorrectly(decimal rate, decimal original, decimal expected)
    {
        var discount = new Discount { DiscountType = DiscountType.Value, Rate = rate };
        Assert.Equal(expected, discount.Apply(original));
    }

    [Fact]
    public void Apply_ShouldNeverReturnNegative()
    {
        var discount = new Discount { DiscountType = DiscountType.Value, Rate = -9999.99m };
        Assert.Equal(0, discount.Apply(100m));
    }

    [Fact]
    public void Apply_ShouldReturnOriginalValue_ForUnknownDiscountType()
    {
        var discount = new Discount { DiscountType = (DiscountType)99, Rate = 0.50m };
        Assert.Equal(100m, discount.Apply(100m));
    }
}
