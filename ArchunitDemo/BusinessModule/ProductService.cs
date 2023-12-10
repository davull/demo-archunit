using System.Collections.Immutable;
using ArchunitDemo.DataModule;

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
        var date = DateTime.UtcNow.AddDays(-14);

        return _productRepository.GetAllProducts()
            .Where(row => row.CreatedAt >= date)
            .Select(row => row.ToProduct())
            .ToImmutableList();
    }
}