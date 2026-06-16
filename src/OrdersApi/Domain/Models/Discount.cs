using OrdersApi.Domain.Enums;

namespace OrdersApi.Domain.Models;

public class Discount
{
    public Guid Id { get; set; }
    public OrderType OrderType { get; set; }
    public DiscountType DiscountType { get; set; }

    /// <summary>
    /// Positive = surcharge, negative = discount.
    /// Percentage: 0.15 = +15%, -0.10 = -10%.
    /// Value: 10.00 = +$10, -10.00 = -$10.
    /// </summary>
    public decimal Rate { get; set; }

    public bool Active { get; set; }

    public decimal Apply(decimal originalValue)
    {
        var result = DiscountType switch
        {
            DiscountType.Percentage => originalValue * (1 + Rate),
            DiscountType.Value      => originalValue + Rate,
            _                       => originalValue
        };
        return Math.Max(0, result);
    }
}
