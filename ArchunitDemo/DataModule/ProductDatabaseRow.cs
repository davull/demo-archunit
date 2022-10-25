namespace ArchunitDemo.DataModule;

public class ProductDatabaseRow
{
    public string ProductName { get; init; } = default!;
    public string ProductDescription { get; init; } = default!;
    public decimal EuroPrice { get; init; }
}