using GhdCodingTest.Models;

namespace GhdCodingTest.Repository;

public interface IProductRepository
{
    Task<Product?> GetProduct(int id);

    Task<bool> ProductExistsByNameAndBrand(string name, string brand);

    Task<IEnumerable<Product>> GetProducts();

    Task<Product> CreateProduct(string name, string brand, int price);

    Task<Product?> UpdateProduct(int id, string name, string brand, int price);

    Task DeleteProduct(Product product);
}