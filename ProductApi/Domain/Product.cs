using ProductApi.Infraestructure;

namespace ProductApi.Domain;

public class Product
{
    public required Guid Id { get; set; }
    public required string Name { get; init; }
    public required int Status { get; init; }
    public required int Stock { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }

    public static Product Create(string name, int status, int stock, string description, decimal price)
    { 
        return new Product {
            Id = Guid.NewGuid(), 
            Name = name, 
            Status = status, 
            Stock =stock, 
            Description= description, 
            Price = price };
    }
}

public static class ProductExtensions
{
    public static ProductResponseDto ToProductResponseDto(this Product product, string statusName, int discount)
    {
        return new ProductResponseDto(
            product.Id,
            product.Name,
            statusName,
            product.Stock,
            product.Description,
            product.Price,
            discount,
            product.Price * (100 - discount) / 100
        );
    }
}