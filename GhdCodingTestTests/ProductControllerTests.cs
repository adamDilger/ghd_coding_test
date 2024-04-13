using System.Data.Common;
using System.Net;
using GhdCodingTest;
using GhdCodingTest.Controllers;
using GhdCodingTest.Models;
using GhdCodingTest.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhdCodingTestTests;

[TestClass]
public class ProductControllerTests
{
    [TestMethod]
    public async Task GetProducts_returnsList()
    {
        var mockRepo = new Mock<IProductRepository>();
        mockRepo.Setup(d => d.GetProducts()).ReturnsAsync(GetTestProducts());

        var controller = new ProductController(Mock.Of<ILogger<ProductController>>(), mockRepo.Object);

        var contentResult = (await controller.GetProducts()).Result as OkObjectResult;
        Assert.IsNotNull(contentResult);

        var response = contentResult.Value as IEnumerable<Product>;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Count());
    }

    [TestMethod]
    public async Task GetProduct_returnsCorrectProduct()
    {
        var mockRepo = new Mock<IProductRepository>();
        mockRepo.Setup(d => d.GetProduct(1)).ReturnsAsync(GetTestProducts().First());

        var controller = new ProductController(Mock.Of<ILogger<ProductController>>(), mockRepo.Object);

        var contentResult = (await controller.GetProduct(1)).Result as OkObjectResult;
        Assert.IsNotNull(contentResult);

        var response = contentResult.Value as Product;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Id);
    }

    [TestMethod]
    public async Task CreateProduct_throwsConflictOnSameName()
    {
        var mockRepo = new Mock<IProductRepository>();
        var products = GetTestProducts();

        mockRepo.Setup(d => d.ProductExistsByNameAndBrand("existing", "existing")).ReturnsAsync(true);
        mockRepo.Setup(d => d.ProductExistsByNameAndBrand("testName", "testBrand")).ReturnsAsync(false);

        var controller = new ProductController(Mock.Of<ILogger<ProductController>>(), mockRepo.Object);

        var requestBody = new ProductController.CreateUpdateProductRequestBody()
        {
            Brand = "existing",
            Name = "existing",
            Price = 100
        };
        var contentConflictResult = (await controller.CreateProduct(requestBody)).Result as ConflictResult;
        Assert.IsNotNull(contentConflictResult);
        Assert.AreEqual((int)HttpStatusCode.Conflict, contentConflictResult.StatusCode);

        requestBody.Name = "testName";
        requestBody.Brand = "testBrand";
        mockRepo.Setup(d => d.CreateProduct(requestBody.Name, requestBody.Brand, requestBody.Price)).ReturnsAsync(new Product() { Id = 1 });
        var contentResult = (await controller.CreateProduct(requestBody)).Result as OkObjectResult;
        Assert.IsNotNull(contentResult);
        Assert.AreEqual((int)HttpStatusCode.OK, contentResult.StatusCode);
        Assert.AreEqual(contentResult.Value, 1);
    }

    [TestMethod]
    public async Task DeleteProduct_worksAndReturns404()
    {
        var mockRepo = new Mock<IProductRepository>();

        var products = GetTestProducts();

        mockRepo.Setup(d => d.GetProduct(1)).ReturnsAsync(products.First());

        var controller = new ProductController(Mock.Of<ILogger<ProductController>>(), mockRepo.Object);

        var contentNoResult = (await controller.DeleteProduct(404)) as NotFoundResult;
        Assert.IsNotNull(contentNoResult);
        Assert.AreEqual(404, contentNoResult.StatusCode);

        var contentResult = (await controller.DeleteProduct(1)) as NoContentResult;
        mockRepo.Verify(d => d.DeleteProduct(products.First()), Times.Once);
        Assert.IsNotNull(contentResult);
        Assert.AreEqual((int)HttpStatusCode.NoContent, contentResult.StatusCode);
    }

    private List<Product> GetTestProducts()
    {
        var products = new List<Product>
        {
            new Product()
            {
                Id = 1,
                Name = "Test One",
                Brand = "Brand One",
            },
            new Product()
            {
                Id = 2,
                Name = "Test Two",
                Brand = "Brand Two",
            }
        };

        return products;
    }
}