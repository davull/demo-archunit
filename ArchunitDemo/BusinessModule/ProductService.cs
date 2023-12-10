using System.Collections.Immutable;
using ArchunitDemo.DataModule;
using Microsoft.Data.SqlClient;

namespace ArchunitDemo.BusinessModule;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService()
    {
        _productRepository = new ProductRepository();
    }

    public IReadOnlyCollection<Product> GetAllProducts()
    {
        return _productRepository.GetAllProducts()
            .Select(row => row.ToProduct())
            .ToImmutableList();
    }

    public IReadOnlyCollection<Product> GetProductsFromLastTwoWeeks()
    {
        var date = DateTime.Now.AddDays(-14);

        return _productRepository.GetAllProducts()
            .Where(row => row.CreatedAt >= date)
            .Select(row => row.ToProduct())
            .ToImmutableList();
    }

    public IReadOnlyCollection<Product> GetProductsByName(string name)
    {
        using var con = new SqlConnection("Server=.;Database=ArchunitDemo;Trusted_Connection=True;");
        con.Open();

        using var cmd = new SqlCommand("SELECT * FROM Products WHERE Name = @Name", con);
        cmd.Parameters.AddWithValue("@Name", name);

        using var reader = cmd.ExecuteReader();
        var products = new List<Product>();

        while (reader.Read())
        {
            var productName = reader.GetString(0);
            var description = reader.GetString(1);
            var price = reader.GetDecimal(2);
            var product = new Product(productName, description, price);
            products.Add(product);
        }

        return products;
    }
}