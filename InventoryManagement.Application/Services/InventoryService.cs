using System.Text.Json;
using FlashSaleDB.Entities;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Mapster;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace InventoryManagement.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<InventoryService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDatabase _cache;

    public InventoryService(IInventoryRepository inventoryRepository, IConnectionMultiplexer redis,
        ICartRepository cartRepository, ILogger<InventoryService> logger, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _cartRepository = cartRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
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

    public async Task UpdateInventory(string paymentSuccessMessage)
    {
        var transaction = _unitOfWork.BeginTransaction();
        
        try
        {
            var data = JsonSerializer.Deserialize<PaymentSuccessMessage>(paymentSuccessMessage,
                JsonSerializerOptions.Web);

            if (data is null) return;

            var cartId = await _cartRepository.GetCartId(Guid.Parse(data?.OrderId));

            var cart = await _cartRepository.GetCart(cartId);

            await UpdateProductsQuantities(cart.CartItems);

            await _unitOfWork.SaveChangesAsync();
            transaction.Commit();
            
            _logger.LogInformation($"Successfully updated the inventory for the Order Id: {data?.OrderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to Update the inventory. Message: {ex.Message}");
            transaction.Rollback();
            throw;
        }
    }

    private async Task UpdateProductsQuantities(List<CartItemModel> cartItems)
    {
        foreach (var cartItem in cartItems)
        {
            var inventoryModel = await _inventoryRepository.GetInventoryByProductId(Guid.Parse(cartItem.ProductId));

            var quantityAfterDecrement = inventoryModel.AvailableQuantity - cartItem.Quantity;

            if (quantityAfterDecrement < 0) throw new Exception("Insufficient stock");

            var inventory = inventoryModel.Adapt<Inventory>();
            inventory.AvailableQuantity = quantityAfterDecrement;
            inventory.UpdatedAt = DateTime.UtcNow;

            _inventoryRepository.UpdateProductQuantity(inventory);
        }
    }
}