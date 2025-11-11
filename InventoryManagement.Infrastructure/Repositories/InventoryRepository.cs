using FlashSaleDB;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly FlashSaleDbContext _context;

    public InventoryRepository(FlashSaleDbContext context)
    {
        _context = context;
    }

    public async Task<List<CacheProduct>> GetInventoryAsync()
    {
        return await _context.Product
            .Include(p => p.Inventory)
            .Select(p => new CacheProduct
            {
                Id = p.Id,
                // Inventory might be null → 0, otherwise take the value
                AvailableQuantity = p.Inventory != null
                    ? p.Inventory.AvailableQuantity
                    : 0
            })
            .ToListAsync();
    }

    public async Task<List<ProductModel>> GetProductsAsync(int page, int pageSize)
    {
        return await _context.Product
            .OrderBy(p => p.Name)                    // order first
            .Skip(page * pageSize)                   // then page
            .Take(pageSize)
            .Select(p => new ProductModel
            {
                ProductId = p.Id,
                Name = p.Name,
                Price = p.Price,
                SaleId = p.SaleId,
                ImageUrl = p.ImageUrl,
                Category = p.Category,
                Description = p.Description,
            })
            .ToListAsync();
    }
}
