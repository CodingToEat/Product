using FluentValidation;
using FluentValidation.AspNetCore;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Moq;
using Moq.Protected;
using ProductApi.Application;
using ProductApi.Domain;
using ProductApi.Infraestructure;
using ProductApi.Infrastructure;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IProductStatusCacheService, ProductStatusCacheService>();

builder.Services.AddSingleton<IAppCache, CachingService>();

builder.Services.AddHttpClient("DiscountApi", client =>
{
    client.BaseAddress = new Uri("https://mockapi.com/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
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
             await Task.Delay(random.Next(100, 500), cancellationToken);
             var discount = random.Next(0, 100);
             var response = new HttpResponseMessage
             {
                 StatusCode = HttpStatusCode.OK,
                 Content = new StringContent($"{{\"Discount\": {discount}}}")
             };
             return response;
         });

    return handlerMock.Object;
});

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductApi", Version = "v1" });
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", 
    rollingInterval: RollingInterval.Day, 
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddTransient<IValidator<ProductRequestDto>, ProductRequestDtoValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapPost("/products", async (ProductService service, ProductRequestDto productDto) =>
{
    var result = await service.AddProductAsync(productDto);
    return Results.Created($"/products/{result.Id}", result);
});

app.MapPut("/products/{id}", async (ProductService service, IProductRepository repository,  Guid id, ProductRequestDto productDto) =>
{
    var product = await repository.GetByIdAsync(id);
    if (product is null) return Results.NotFound();

    await service.UpdateProductAsync(product, productDto);
    return Results.NoContent();
});

app.MapGet("/products/{id}", async (ProductService service, Guid id) =>
{
    var product = await service.GetProductByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.Run();