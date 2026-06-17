using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Requests;

public record UpdateProductRequest(
    [Required]
    string Name,
    [Required]
    string ImageURL,
    [Required]
    string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    decimal Price);
