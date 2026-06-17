using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Requests;

public record UpdateProductRequest(
    [property: Required]
    string Name,
    [property: Required]
    string ImageURL,
    [property: Required]
    string Description,
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    decimal Price);
