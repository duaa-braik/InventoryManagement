using FlashSaleDB;
using FlashSaleDB.Entities;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Infrastructure.Repositories;

public class CartRespository : ICartRepository
{
    private readonly FlashSaleDbContext _context;
    
    public CartRespository(FlashSaleDbContext context)
    {
        _context = context;    
    }

    public void CreateCart(Cart cart)
    {
        _context.Cart.Add(cart);
    }

    public void AddItemToCart(CartItem cartItem)
    {
        _context.CartItem.Add(cartItem);
    }
}