using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using StackExchange.Redis;

namespace InventoryManagement.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IDatabase _cache;
    
    public InventoryService(IInventoryRepository inventoryRepository, IConnectionMultiplexer redis)
    {
        _inventoryRepository = inventoryRepository;
        _cache = redis.GetDatabase();
    }

    public async Task<List<ProductModel>> GetProducts(int page, int pageSize)
    {
        var pageNumber = page == 0 ? 1 : page;
        var size = pageSize == 0 ? 10 : pageSize;

        var products = await _inventoryRepository.GetProductsAsync(pageNumber, size);
        
        products.ForEach(product =>
        {
            var keyName = $"inventory:{product.ProductId}";
            var availableStock = _cache.StringGet(keyName);
            
            product.AvailableStock = int.TryParse(availableStock.ToString(), out int availableQuantity) ? availableQuantity : 0;
        });

        return products;
    }
}