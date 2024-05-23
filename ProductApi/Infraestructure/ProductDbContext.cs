using ProductApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace ProductApi.Infrastructure;

    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }