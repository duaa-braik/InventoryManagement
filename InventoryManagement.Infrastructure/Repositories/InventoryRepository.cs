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
                    AvailableQuantity = p.Inventory.AvailableQuantity
                }
            )
            .ToListAsync();
    }
}