using FlashSaleDB;
using FlashSaleDB.Entities;

namespace InventoryManagement.Infrastructure.Repositories;

public class CartRespository
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
}