using ServiceRepoApi.Models;

namespace ServiceRepoApi.Repositories;

// Repository = data access only. No business rules here.
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<bool> NameExistsAsync(string name, int? excludeId = null);

    Task AddAsync(Product product);

    void Update(Product product);

    void Delete(Product product);

    Task SaveChangesAsync();
}