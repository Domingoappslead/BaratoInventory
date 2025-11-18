using BaratoInventory.Core.Entities;
using BaratoInventory.Core.Interfaces;
using BaratoInventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaratoInventory.Infrastructure.Repositories;

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
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id) ?? throw new KeyNotFoundException($"Product with ID {product.Id} not found");

        existingProduct.Name = product.Name;
        existingProduct.Category = product.Category;
        existingProduct.Price = product.Price;
        existingProduct.Quantity = product.Quantity;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var lowerSearchTerm = searchTerm.ToLower().Trim();

        return await _context.Products
            .Where(p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                p.Category.ToLower().Contains(lowerSearchTerm) ||
                p.Price.ToString().Contains(lowerSearchTerm) ||
                p.Quantity.ToString().Contains(lowerSearchTerm) ||
                p.Id.ToString().Contains(lowerSearchTerm))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

}
