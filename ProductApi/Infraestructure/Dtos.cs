using FluentValidation;
using ProductApi.Domain;

namespace ProductApi.Infraestructure;

public record ProductResponseDto(
    Guid ProductId,
    string Name,
    string StatusName,
    int Stock,
    string Description,
    decimal Price,
    int Discount,
    decimal FinalPrice);

public record ProductRequestDto(
    string Name,
    int Status,
    int Stock,
    string Description,
    decimal Price
    )
{
    public static implicit operator Product(ProductRequestDto dto) =>
               Product.Create(dto.Name, dto.Status, dto.Stock, dto.Description, dto.Price);
}

public record DiscountResponseDto(int Discount);





