using FlashSaleDB.Entities;
using InventoryManagement.Application.Dtos;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Mapster;

namespace InventoryManagement.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        
    }

    public async Task<CartModel> CreateCartAsync(CreateCartRequest cartRequest)
    {
        var transaction = _unitOfWork.BeginTransaction();

        try
        {
            var cart = cartRequest.Adapt<Cart>();
            _cartRepository.CreateCart(cart);
            await _unitOfWork.SaveChangesAsync();
            transaction.Commit();

            return cart.Adapt<CartModel>();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}