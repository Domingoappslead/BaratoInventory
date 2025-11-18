using BaratoInventory.Core.Entities;
using BaratoInventory.Core.Interfaces;
using BaratoInventory.Infrastructure.Services;
using Moq;

namespace BaratoInventory.Tests.Services;

[TestFixture]
public class ProductServiceTests
{
    private Mock<IProductRepository> _mockRepository;
    private Mock<ICacheService> _mockCacheService;
    private ProductService _productService;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _productService = new ProductService(_mockRepository.Object, _mockCacheService.Object);
    }

    [Test]
    public async Task GetAllProductsAsync_WhenCacheIsEmpty_ShouldReturnProductFromDatabase()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Quantity = 10 },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 25.50m, Quantity = 50 }
        };

        _mockCacheService
            .Setup(x => x.GetAsync<List<Product>>(It.IsAny<string>()))
            .ReturnsAsync((List<Product>?)null);

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().Name, Is.EqualTo("Laptop"));

        // Verify cache was checked
        _mockCacheService.Verify(x => x.GetAsync<List<Product>>(It.IsAny<string>()), Times.Once);

        // Verify database was queried
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);

        // Verify data was stored in cache
        _mockCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<Product>>(), It.IsAny<TimeSpan?>()), Times.Once);
    }
    [Test]
    public async Task GetAllProductsAsync_WhenCacheHasData_ShouldReturnCachedProducts()
    {
        // Arrange
        var cachedProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Cached Laptop", Category = "Electronics", Price = 999.99m, Quantity = 10 }
        };

        _mockCacheService
            .Setup(x => x.GetAsync<List<Product>>(It.IsAny<string>()))
            .ReturnsAsync(cachedProducts);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Cached Laptop"));

        // Verify cache was checked
        _mockCacheService.Verify(x => x.GetAsync<List<Product>>(It.IsAny<string>()), Times.Once);

        // Verify database was NOT queried (cache hit)
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Test]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Quantity = 10 };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product?)null);

        _mockRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Laptop"));

        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task CreateProductAsync_ShouldCreateProductAndInvalidateCache()
    {
        // Arrange
        var newProduct = new Product { Name = "New Product", Category = "Test", Price = 100m, Quantity = 5 };
        var createdProduct = new Product { Id = 10, Name = "New Product", Category = "Test", Price = 100m, Quantity = 5 };

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateProductAsync(newProduct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(10));
        Assert.That(result.Name, Is.EqualTo("New Product"));

        // Verify product was added
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);

        // Verify cache was invalidated
        _mockCacheService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task UpdateProductAsync_ShouldUpdateProductAndInvalidateCache()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Updated Laptop", Category = "Electronics", Price = 1200m, Quantity = 8 };

        _mockRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.UpdateProductAsync(product);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Updated Laptop"));
        Assert.That(result.Price, Is.EqualTo(1200m));

        // Verify update was called
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);

        // Verify cache was invalidated (should be called twice: once for list, once for specific product)
        _mockCacheService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(2));
    }

    [Test]
    public async Task DeleteProductAsync_WhenProductExists_ShouldDeleteAndInvalidateCache()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteProductAsync(1);

        // Assert
        Assert.That(result, Is.True);

        // Verify delete was called
        _mockRepository.Verify(x => x.DeleteAsync(1), Times.Once);

        // Verify cache was invalidated
        _mockCacheService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(2));
    }

    [Test]
    public async Task SearchProductsAsync_ShouldReturnSearchResults()
    {
        // Arrange
        var searchTerm = "laptop";
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Quantity = 10 },
            new Product { Id = 2, Name = "Gaming Laptop", Category = "Electronics", Price = 1500m, Quantity = 5 }
        };

        _mockRepository
            .Setup(x => x.SearchAsync(searchTerm))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.SearchProductsAsync(searchTerm);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));

        _mockRepository.Verify(x => x.SearchAsync(searchTerm), Times.Once);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test if needed
    }
}
