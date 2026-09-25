using POS.Domain.Entities;
namespace POS.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByProductCodeAsync(string sku, CancellationToken cancellationToken = default);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
}