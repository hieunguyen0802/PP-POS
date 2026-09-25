using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;

namespace POS.Infrastructure.Repositories;

public class ProductRepository : IProductRepository

{
    private readonly PosDBContext _context;
    public ProductRepository(PosDBContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByProductCodeAsync(string ProductCode, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductCode == ProductCode, cancellationToken);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductBarcode == barcode, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
    }


    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(entity, cancellationToken);
    }

    public void Update(Product entity)
    {
        _context.Products.Update(entity);
    }


    public void Delete(Product entity)
    {
        _context.Products.Remove(entity);
    }


}