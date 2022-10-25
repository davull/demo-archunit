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
}