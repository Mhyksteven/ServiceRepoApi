using Microsoft.EntityFrameworkCore;
using ServiceRepoApi.Data;
using ServiceRepoApi.Models;

namespace ServiceRepoApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        return await _context.Products
            .AnyAsync(p =>
                p.Name == name &&
                (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}