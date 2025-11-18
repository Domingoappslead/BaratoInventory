using BaratoInventory.Core.Entities;
using BaratoInventory.Core.Interfaces;

namespace BaratoInventory.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cacheService;
    private const string ProductCacheKey = "products:all";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

    public ProductService(IProductRepository repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var cachedProducts = await _cacheService.GetAsync<List<Product>>(ProductCacheKey);

        if (cachedProducts != null && cachedProducts.Any())
            return cachedProducts;

        var products = await _repository.GetAllAsync();
        var productList = products.ToList();

        if (productList.Any())
        {
            await _cacheService.SetAsync(ProductCacheKey, productList, _cacheExpiration);
        }
        return productList;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"products:{id}";

        var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
            return cachedProduct;

        var product = await _repository.GetByIdAsync(id);

        if (product != null)
        {
            await _cacheService.SetAsync(cacheKey, product, _cacheExpiration);
        }

        return product;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        var createdProduct =  _repository.AddAsync(product);

        await InvalidateCacheAsync();

        return await createdProduct;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var updatedProduct = await _repository.UpdateAsync(product);

        await InvalidateCacheAsync();
        await _cacheService.RemoveAsync($"product:{product.Id}");

        return updatedProduct;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);

        if (result)
        {
            await InvalidateCacheAsync();
            await _cacheService.RemoveAsync($"product:{id}");
        }

        return result;
    }

    public async Task ClearCacheAsync()
    {
        await InvalidateCacheAsync();
    }

    private async Task InvalidateCacheAsync()
    {
        await _cacheService.RemoveAsync(ProductCacheKey);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        // For search,skip cache and query directly
        // This ensures fresh results for dynamic queries
        return await _repository.SearchAsync(searchTerm);
    }
}
