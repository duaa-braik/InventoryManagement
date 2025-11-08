using StackExchange.Redis;

namespace InventoryManagement.Application.Interfaces;

public interface IInventoryCacheService
{
    Task LoadInventoryIntoCache();

    Task DecrementQuantityAsync(int quantityToDecrement, string productId, ITransaction transaction);
}