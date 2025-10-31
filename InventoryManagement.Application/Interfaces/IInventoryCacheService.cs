namespace InventoryManagement.Application.Interfaces;

public interface IInventoryCacheService
{
    Task LoadInventoryIntoCache();
}