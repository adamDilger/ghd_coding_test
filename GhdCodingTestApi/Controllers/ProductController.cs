using Microsoft.AspNetCore.Mvc;
using GhdCodingTest.Models;
using GhdCodingTest.Repository;

namespace GhdCodingTest.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    private readonly IProductRepository _productRepository;

    public ProductController(ILogger<ProductController> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    [HttpGet("/products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        _logger.LogInformation("Getting products");
        var products = await _productRepository.GetProducts();
        return Ok(products);
    }

    [HttpGet("/products/{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        _logger.LogInformation("Getting product " + id);
        return Ok(await _productRepository.GetProduct(id));
    }

    [HttpPost("/products")]
    public async Task<ActionResult<int>> CreateProduct(CreateUpdateProductRequestBody body)
    {
        _logger.LogInformation("Creating product name: " + body.Name + ", brand: " + body.Brand);
        if (await _productRepository.ProductExistsByNameAndBrand(body.Name, body.Brand))
        {
            return Conflict();
        }

        var product = await _productRepository.CreateProduct(body.Name, body.Brand, body.Price);
        return Ok(product.Id);
    }

    [HttpPut("/products/{id}")]
    public async Task<ActionResult> UpdateProduct(int id, CreateUpdateProductRequestBody body)
    {
        _logger.LogInformation("Updating product name: " + body.Name + ", brand: " + body.Brand);

        var productToUpdate = await _productRepository.UpdateProduct(id, body.Name, body.Brand, body.Price);
        if (productToUpdate == null) return NotFound();

        return NoContent();
    }


    public class CreateUpdateProductRequestBody
    {
        public required string Name { get; set; }
        public required string Brand { get; set; }
        public required int Price { get; set; }
    }


    [HttpDelete("/products/{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product " + id);

        var product = await _productRepository.GetProduct(id);
        if (product == null) return NotFound();

        await _productRepository.DeleteProduct(product);

        return NoContent();
    }
}
