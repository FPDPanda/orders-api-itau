using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Requests;

public record UpdateProductRequest(
    [param: Required]
    string Name,
    [param: Required]
    string ImageURL,
    [param: Required]
    string Description,
    [param: Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    decimal Price);
