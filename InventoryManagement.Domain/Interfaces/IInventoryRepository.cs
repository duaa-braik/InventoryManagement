using InventoryManagement.Domain.Models;

namespace InventoryManagement.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<List<CacheProduct>> GetInventoryAsync();
}