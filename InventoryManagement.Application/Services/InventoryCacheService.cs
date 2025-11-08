using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using StackExchange.Redis;

namespace InventoryManagement.Application.Services;

public class InventoryCacheService : IInventoryCacheService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IDatabase _cache;
    
    public InventoryCacheService(IInventoryRepository inventoryRepository, IConnectionMultiplexer redis)
    {
        _inventoryRepository = inventoryRepository;
        _cache = redis.GetDatabase();
    }

    public async Task LoadInventoryIntoCache()
    {
        Console.WriteLine("Loading inventory data into Redis...");
        
        var products = await _inventoryRepository.GetInventoryAsync();
        
        var batch = _cache.CreateBatch();

        foreach (var product in products)
        {
            var key = $"inventory:{product.Id}";
            batch.StringSetAsync(key, product.AvailableQuantity);
        }
        
        batch.Execute();
        
        Console.WriteLine($"Loaded {products.Count} inventory items into Redis.");
    }

    public async Task DecrementQuantityAsync(int quantityToDecrement, string productId, ITransaction transaction)
    {
        var productKey = $"inventory:{productId.ToLower()}";

        var productValue = transaction.StringGetAsync(productKey);
        
        await transaction.ExecuteAsync();

        var currentQuantity = int.TryParse(productValue.Result.ToString(), out var quantity)
            ? quantity
            : 0;

        var result = currentQuantity - quantityToDecrement;

        if (result < 0) throw new Exception("Item is out of stock");
        
        transaction.StringDecrementAsync(productKey, quantityToDecrement);
    }
}