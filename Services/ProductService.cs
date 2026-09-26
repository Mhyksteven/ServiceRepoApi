using ServiceRepoApi.Models;
using ServiceRepoApi.Repositories;

namespace ServiceRepoApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public async Task<ServiceResult> CreateAsync(Product product)
    {
        product.Name = product.Name.Trim();

        if (await _repository.NameExistsAsync(product.Name))
        {
            return ServiceResult.Conflict(
                $"A product named '{product.Name}' already exists.");
        }

        product.Id = 0;
        product.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(Product product)
    {
        var existing = await _repository.GetByIdAsync(product.Id);

        if (existing is null)
        {
            return ServiceResult.NotFound("Product not found.");
        }

        product.Name = product.Name.Trim();

        if (await _repository.NameExistsAsync(product.Name, product.Id))
        {
            return ServiceResult.Conflict(
                $"A product named '{product.Name}' already exists.");
        }

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.Stock = product.Stock;

        _repository.Update(existing);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
        {
            return ServiceResult.NotFound("Product not found.");
        }

        if (product.Stock > 0)
        {
            return ServiceResult.Conflict(
                "Cannot delete a product that still has stock.");
        }

        _repository.Delete(product);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }
}