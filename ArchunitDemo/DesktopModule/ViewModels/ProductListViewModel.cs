using System.Collections.ObjectModel;
using ArchunitDemo.BusinessModule;

namespace ArchunitDemo.DesktopModule.ViewModels;

public class ProductListViewModel
{
    public ObservableCollection<Product> Products { get; }

    public ProductListViewModel()
    {
        var productService = new ProductService();
        Products = new ObservableCollection<Product>(productService.GetAllProducts());
    }
}