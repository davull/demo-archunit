using ArchunitDemo.DataModule;

namespace ArchunitDemo.BusinessModule;

public class Product
{
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }

    public Product(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
    }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Price)}: {Price}";
    }
}

internal static class ProductExtensions
{
    public static Product ToProduct(this ProductDatabaseRow databaseRow) =>
        new(
            name: databaseRow.ProductName,
            description: databaseRow.ProductDescription,
            price: databaseRow.EuroPrice);
} 
