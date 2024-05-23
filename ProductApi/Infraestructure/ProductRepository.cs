using Microsoft.EntityFrameworkCore;
using ProductApi.Domain;

namespace ProductApi.Infrastructure;
public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id) => await _context.Products.FindAsync(id);

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Attach(product);
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public void Detach(Product product)
    {
        _context.Entry(product).State = EntityState.Detached;
    }
}