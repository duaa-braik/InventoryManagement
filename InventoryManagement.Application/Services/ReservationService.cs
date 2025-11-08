using System.Text.Json;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using StackExchange.Redis;

namespace InventoryManagement.Application.Services;

public class ReservationService : IReservationService
{
    private readonly ICartRepository _cartRepository;
    private readonly IInventoryCacheService _inventoryCacheService;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(IConnectionMultiplexer redis, ICartRepository cartRepository,
        IInventoryCacheService inventoryCacheService, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _inventoryCacheService = inventoryCacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessReservation(string message)
    {
        try
        {
            var reservation = JsonSerializer.Deserialize<ReservationMessage>(message, options: JsonSerializerOptions.Web);
            var orderId = Guid.Parse(reservation?.OrderId ?? Guid.Empty.ToString());

            var cartId = await _cartRepository.GetCartId(orderId);

            var cart = await _cartRepository.GetCart(cartId);
            var transaction = _unitOfWork.CreateRedisTransaction();

            foreach (var cartItem in cart.CartItems)
            {
                await _inventoryCacheService.DecrementQuantityAsync(cartItem.Quantity, cartItem.ProductId, transaction);
            }

            await transaction.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}