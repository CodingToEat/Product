using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using ProductApi.Application;
using ProductApi.Domain;
using ProductApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using ProductApi.Infraestructure;

namespace ProductApi.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ProductDbContext _context;
        private readonly IProductRepository _repository;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IProductStatusCacheService> _statusCacheServiceMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test
                .Options;

            _context = new ProductDbContext(options);
            _repository = new ProductRepository(_context);

            var handlerMock = new Mock<HttpMessageHandler>();
            var random = new Random();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(async (HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    await Task.Delay(100); // Simulate latency
                    var discount = random.Next(0, 100);
                    var response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent($"{{\"discount\": {discount}}}")
                    };
                    return response;
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://mockapi.com/api/")
            };

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _statusCacheServiceMock = new Mock<IProductStatusCacheService>();
            _statusCacheServiceMock.Setup(s => s.GetStatusesAsync())
                                   .ReturnsAsync(new Dictionary<int, string> { { 1, "Active" }, { 0, "Inactive" } });

            _service = new ProductService(_repository,  _httpClientFactoryMock.Object, _statusCacheServiceMock.Object);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProduct()
        {
            // Arrange
            var productDto = new ProductRequestDto("Test Product", 1, 10, "Test Description", 100.0m);

            // Act
            await _service.AddProductAsync(productDto);

            // Assert
            var products = await _context.Products.ToListAsync();
            Assert.Single(products);
            var product = products.First();
            Assert.Equal(productDto.Name, product.Name);
            Assert.Equal(productDto.Status, product.Status);
            Assert.Equal(productDto.Stock, product.Stock);
            Assert.Equal(productDto.Description, product.Description);
            Assert.Equal(productDto.Price, product.Price);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductResponseDto()
        {
            // Arrange
            var product = Product.Create("Test Product", 1, 10, "Test Description", 100.0m);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.ProductId);
            Assert.Equal(product.Name, result.Name);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            var product = Product.Create("Test Product", 1, 10, "Test Description", 100.0m);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductRequestDto("Updated Product", 0, 20, "Updated Description", 200.0m);

            // Act
            await _service.UpdateProductAsync(product, productDto);

            // Assert
            var updatedProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(productDto.Name, updatedProduct.Name);
            Assert.Equal(productDto.Status, updatedProduct.Status);
            Assert.Equal(productDto.Stock, updatedProduct.Stock);
            Assert.Equal(productDto.Description, updatedProduct.Description);
            Assert.Equal(productDto.Price, updatedProduct.Price);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
