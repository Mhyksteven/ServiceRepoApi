using ServiceRepoApi.Models;

namespace ServiceRepoApi.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<ServiceResult> CreateAsync(Product product);
    Task<ServiceResult> UpdateAsync(Product product);
    Task<ServiceResult> DeleteAsync(int id);
}

public enum ServiceResultStatus
{
    Ok,
    NotFound,
    Conflict
}

public record ServiceResult(ServiceResultStatus Status, string? Error = null)
{
    public bool Success => Status == ServiceResultStatus.Ok;

    public static ServiceResult Ok()
        => new(ServiceResultStatus.Ok);

    public static ServiceResult NotFound(string error)
        => new(ServiceResultStatus.NotFound, error);

    public static ServiceResult Conflict(string error)
        => new(ServiceResultStatus.Conflict, error);
}