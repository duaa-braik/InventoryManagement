using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.API.Utils;

public class Utils
{
    public static async Task LoadInventory(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryCacheService>();
        await inventoryService.LoadInventoryIntoCache();
    }
}