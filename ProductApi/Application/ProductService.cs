using ProductApi.Domain;
using ProductApi.Infraestructure;
using System.Text.Json;

namespace ProductApi.Application;
public class ProductService(IProductRepository repository, IHttpClientFactory httpClientFactory, IProductStatusCacheService statusCacheService)
{
    private readonly IProductRepository _repository = repository;
    private readonly IProductStatusCacheService _statusCacheService = statusCacheService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return null;

        var statuses = await _statusCacheService.GetStatusesAsync();
        var statusName = statuses.ContainsKey(product.Status) ? statuses[product.Status] : "Unknown";

        var discount = await GetDiscountAsync(product.Id);

        return product.ToProductResponseDto(statusName, discount);
    }

    private async Task<int> GetDiscountAsync(Guid productId)
    {
        var client = _httpClientFactory.CreateClient("DiscountApi");
        var response = await client.GetAsync("discount");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var discountResponse = JsonSerializer.Deserialize<DiscountResponseDto>(content);
            return discountResponse?.Discount ?? 0;
        }

        return 0;
    }

    public async Task<Product> AddProductAsync(ProductRequestDto productDto)
    {
        Product product = productDto;
        await _repository.AddAsync(product);
        return product;
    }

    public async Task UpdateProductAsync(Product product, ProductRequestDto productDto)
    {
        _repository.Detach(product);
        var updatedProduct = (Product)productDto;
        updatedProduct.Id = product.Id;
        await _repository.UpdateAsync(updatedProduct);
    }
}
