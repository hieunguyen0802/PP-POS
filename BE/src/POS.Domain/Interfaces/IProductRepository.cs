using POS.Domain.Entities;
namespace POS.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold, CancellationToken cancellationToken = default);
}