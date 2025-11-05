using FlashSaleDB.Entities;
using InventoryManagement.Application.Dtos;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Mapster;
using StackExchange.Redis;

namespace InventoryManagement.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDatabase _cache;

    public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork, IConnectionMultiplexer redis)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _cache = redis.GetDatabase();
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

            var createdCart = cart.Adapt<CartModel>();
            createdCart.CartId = cart.Id.ToString();
            
            return createdCart;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task AddItemToCartAsync(AddItemToCartRequest request, string cartId, string itemId)
    {
        var transaction = _unitOfWork.BeginTransaction();

        try
        {
            var availableStock = _cache.StringGet($"inventory:{itemId}");
        
            var availableItemQuantity = int.TryParse(availableStock.ToString(), out var availableQuantity) ? availableQuantity : 0;

            if (availableItemQuantity < request.Quantity) return;
            
            var cartItem = request.Adapt<CartItem>();
            cartItem.ProductId = Guid.Parse(itemId);
            cartItem.CartId = Guid.Parse(cartId);
            
            _cartRepository.AddItemToCart(cartItem);

            await _unitOfWork.SaveChangesAsync();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw;
        }
    }
}