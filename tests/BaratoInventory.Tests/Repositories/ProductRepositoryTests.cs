using BaratoInventory.Core.Entities;
using BaratoInventory.Infrastructure.Data;
using BaratoInventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaratoInventory.Tests.Repositories;

[TestFixture]
public class ProductRepositoryTests
{
    private AppDbContext _context;
    private ProductRepository _repository;

    [SetUp]
    public void SetUp()
    {
        // Create InMemory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Quantity = 10 },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 25.50m, Quantity = 50 },
            new Product { Id = 3, Name = "Desk Chair", Category = "Furniture", Price = 199.99m, Quantity = 20 }
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Laptop"));
    }

    [Test]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task AddAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        var newProduct = new Product
        {
            Name = "Keyboard",
            Category = "Electronics",
            Price = 75.00m,
            Quantity = 30
        };

        // Act
        var result = await _repository.AddAsync(newProduct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Name, Is.EqualTo("Keyboard"));

        // Verify it's in the database
        var allProducts = await _repository.GetAllAsync();
        Assert.That(allProducts.Count(), Is.EqualTo(4));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateExistingProduct()
    {
        // Arrange
        var product = await _repository.GetByIdAsync(1);
        Assert.That(product, Is.Not.Null);

        product!.Name = "Updated Laptop";
        product.Price = 1200m;

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Updated Laptop"));
        Assert.That(result.Price, Is.EqualTo(1200m));
        Assert.That(result.UpdatedAt, Is.Not.Null);

        // Verify in database
        var updatedProduct = await _repository.GetByIdAsync(1);
        Assert.That(updatedProduct!.Name, Is.EqualTo("Updated Laptop"));
    }

    [Test]
    public void UpdateAsync_WhenProductDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var nonExistentProduct = new Product
        {
            Id = 999,
            Name = "Non-existent",
            Category = "Test",
            Price = 100m,
            Quantity = 10
        };

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _repository.UpdateAsync(nonExistentProduct));
    }

    [Test]
    public async Task DeleteAsync_WhenProductExists_ShouldDeleteProduct()
    {
        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        Assert.That(result, Is.True);

        // Verify it's deleted
        var deletedProduct = await _repository.GetByIdAsync(1);
        Assert.That(deletedProduct, Is.Null);

        var allProducts = await _repository.GetAllAsync();
        Assert.That(allProducts.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SearchAsync_ShouldReturnMatchingProducts()
    {
        // Act
        var result = await _repository.SearchAsync("electronics");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2)); // Laptop and Mouse
    }

    [Test]
    public async Task SearchAsync_WithEmptyTerm_ShouldReturnAllProducts()
    {
        // Act
        var result = await _repository.SearchAsync("");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task SearchAsync_ByPrice_ShouldReturnMatchingProducts()
    {
        // Act
        var result = await _repository.SearchAsync("999");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Laptop"));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}