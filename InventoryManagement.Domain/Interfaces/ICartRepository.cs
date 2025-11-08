using FlashSaleDB.Entities;
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Domain.Interfaces;

public interface ICartRepository
{
    void CreateCart(Cart cart);

    void AddItemToCart(CartItem cartItem);

    Task<CartModel> GetCart(Guid? cartId);

    Task<Guid?> GetCartId(Guid orderId);
}