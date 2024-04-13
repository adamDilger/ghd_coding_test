namespace GhdCodingTest.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Brand { get; set; } = "";

    // price in cents
    public int Price { get; set; }

    public string PriceString => string.Format("${0:0.##}", Price / 100.0);
}
