using BaratoInventory.Core.Interfaces;
using BaratoInventory.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using BaratoInventory.API.DTOs;

namespace BaratoInventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            var productDtos = products.Select(p => MapToDto(p));

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
        
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(MapToDto(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving product with ID {id}");
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Category = createProductDto.Category,
                Price = createProductDto.Price,
                Quantity = createProductDto.Quantity
            };

            var createProduct = await _productService.CreateProductAsync(product);
            var productDto = MapToDto(createProduct);

            return CreatedAtAction(
                nameof(GetProductById), 
                new { id = productDto.Id }, 
                productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != updateProductDto.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        try
        {
            var product = new Product
            {
                Id = updateProductDto.Id,
                Name = updateProductDto.Name,
                Category = updateProductDto.Category,
                Price = updateProductDto.Price,
                Quantity = updateProductDto.Quantity
            };

            var updatedProduct = await _productService.UpdateProductAsync(product);
            var productDto = MapToDto(updatedProduct);

            return Ok(productDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product with ID {ProductId} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating product with ID {id}");
            return StatusCode(500, "An error occurred while updating the product");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting product with ID {id}");
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }

    private static ProductDto MapToDto(Product product) =>
        new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Quantity = product.Quantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

    // GET: api/products/search?term=laptop
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string term)
    {
        try
        {
            var products = await _productService.SearchProductsAsync(term ?? string.Empty);
            var productDtos = products.Select(p => MapToDto(p));

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", term);
            return StatusCode(500, "An error occurred while searching products");
        }
    }
}
