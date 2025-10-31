using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Services;

public class InventoryCacheService : IInventoryCacheService
{
    private readonly IInventoryRepository _inventoryRepository;
    
    public InventoryCacheService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }
}