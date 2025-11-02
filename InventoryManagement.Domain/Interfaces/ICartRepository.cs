using FlashSaleDB.Entities;

namespace InventoryManagement.Domain.Interfaces;

public interface ICartRepository
{
    void CreateCart(Cart cart);
}