using System.Collections.Immutable;
using Microsoft.Data.SqlClient;

namespace ArchunitDemo.DataModule;

public class ProductRepository
{
    private readonly ICollection<ProductDatabaseRow> _products;

    public ProductRepository()
    {
        _products = new List<ProductDatabaseRow>
        {
            new()
            {
                ProductName = "33.8cm (13.3\") Apple MacBook Air MGN63D/A Space Gray",
                ProductDescription =
                    "Produkttyp: Notebook, Formfaktor: Klappgehäuse. Prozessorfamilie: Apple M, Prozessor: M1. Bildschirmdiagonale: 33, 8 cm (13.3 Zoll), Bildschirmauflösung: 2560 x 1600 Pixel. RAM-Speicher: 8 GB. Gesamtspeicherkapazität: 256 GB, Speichermedien: SSD. On-Board Grafikadaptermodell: Apple GPU. Installiertes Betriebssystem: macOS Big Sur. Produktfarbe: Grau. Gewicht: 1, 29 kg",
                EuroPrice = 1_171.90m,
                CreatedAt = new DateTime(2023, 12, 10)
            },
            new()
            {
                ProductName = "32.8cm (12.9\") Apple MHR63FD/A iPad Pro 12.9\" 5. Gen 256GB 5G Space Gray",
                ProductDescription =
                    "Bildschirmdiagonale: 32, 8 cm (12.9 Zoll), Bildschirmauflösung: 2732 x 2048 Pixel, Bildschirmtechnologie: Mini LED. Interne Speicherkapazität: 256 GB. Prozessorfamilie: Apple M, Prozessor: M1. RAM-Speicher: 8 GB. Auflösung Rückkamera (numerisch): 12 MP, Rückkamera-Typ: Dual-Kamera, Auflösung Frontkamera (numerisch): 12 MP. Top WLAN-Standard: Wi-Fi 6 (802.11ax). Unterstützte Navigationsfunktion (A-GPS). Gewicht: 684 g. Installiertes Betriebssystem: iPadOS 14. Produktfarbe: Grau",
                EuroPrice = 1_543.90m,
                CreatedAt = new DateTime(2023, 12, 01)
            },
            new()
            {
                ProductName = "Apple iMac Retina 4K 21.5\" MNE02D/A",
                ProductDescription =
                    "Produkttyp: All-in-One-PC. Bildschirmdiagonale: 54, 6 cm (21.5 Zoll), HD-Typ: 4K Ultra HD, Bildschirmauflösung: 4096 x 2304 Pixel, Form des Bildschirms: Flach. Prozessorfamilie: Intel® Core i5 der siebten Generation, Prozessor-Taktfrequenz: 3, 4 GHz. RAM-Speicher: 8 GB, Interner Speichertyp: DDR4-SDRAM. Gesamtspeicherkapazität: 1000 GB, Speichermedien: Fusion Drive. Dediziertes Grafikmodell: AMD Radeon Pro 560. Integrierte Kamera. Vorinstalliertes Betriebssystem: macOS Sierra 10.12. Produktfarbe: Silber",
                EuroPrice = 1_461.75m,
                CreatedAt = new DateTime(2023, 04, 20)
            },
        };
    }

    public ProductDatabaseRow? GetProductByName(string name) => _products.FirstOrDefault(p => p.ProductName == name);

    public IReadOnlyCollection<ProductDatabaseRow> GetAllProducts() => _products.ToImmutableList();

    public int GetProductsCount()
    {
        using var con = new SqlConnection("Server=.;Database=ArchunitDemo;Trusted_Connection=True;");
        con.Open();

        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Products", con);
        return (int) cmd.ExecuteScalar();
    }
}