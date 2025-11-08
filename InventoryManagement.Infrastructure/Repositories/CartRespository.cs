using FlashSaleDB;
using FlashSaleDB.Entities;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<CartModel> GetCart(Guid? cartId)
    {
        return await _context.Cart
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .Where(c => c.Id == cartId)
            .Select(c => new CartModel()
            {
                CartId = c.Id.ToString(),
                UserId = c.UserId.ToString(),
                CartItems = c.CartItems.Select(ci => new CartItemModel()
                {
                    ProductId = ci.ProductId.ToString(),
                    Name = ci.Product.Name,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product.ImageUrl,
                    SaleId = ci.Product.SaleId.GetValueOrDefault(),
                }).ToList()
            })
            .FirstAsync();
    }

    public async Task<Guid?> GetCartId(Guid orderId)
    {
        return await _context.Order
            .Where(o => o.Id == orderId)
            .Select(o => o.CartId)
            .FirstAsync();
    }
}