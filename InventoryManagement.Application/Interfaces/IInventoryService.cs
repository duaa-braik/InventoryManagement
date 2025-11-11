using InventoryManagement.Domain.Models;

namespace InventoryManagement.Application.Interfaces;

public interface IInventoryService
{
    Task<List<ProductModel>> GetProducts(int page, int pageSize);

    Task UpdateInventory(string paymentSuccessMessage);
    Task UpdateInventoryForOrderAsync(Guid orderId);
}