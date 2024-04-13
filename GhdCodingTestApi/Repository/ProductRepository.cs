using GhdCodingTest.Models;
using Microsoft.EntityFrameworkCore;

namespace GhdCodingTest.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _productContext;

    public ProductRepository(ProductContext productContext)
    {
        _productContext = productContext;
    }

    public async Task<Product> CreateProduct(string name, string brand, int price)
    {
        var product = new Product { Name = name, Brand = brand, Price = price };

        await _productContext.Products.AddAsync(product);
        await _productContext.SaveChangesAsync();

        return product;
    }

    public async Task DeleteProduct(Product product)
    {
        _productContext.Products.Remove(product);
        await _productContext.SaveChangesAsync();
    }

    public Task<Product?> GetProduct(int id)
    {
        return _productContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<bool> ProductExistsByNameAndBrand(string name, string brand)
    {
        return _productContext.Products.AnyAsync(p => p.Name == name && p.Brand == brand);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _productContext.Products.ToListAsync();
    }

    public async Task<Product?> UpdateProduct(int id, string name, string brand, int price)
    {
        var productToUpdate = await GetProduct(id);
        if (productToUpdate == null) return null;

        productToUpdate.Brand = brand;
        productToUpdate.Name = name;
        productToUpdate.Price = price;

        await _productContext.SaveChangesAsync();
        return productToUpdate;
    }
}