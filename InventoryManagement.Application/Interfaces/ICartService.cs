using InventoryManagement.Application.Dtos;
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Application.Interfaces;

public interface ICartService
{
    Task<CartModel> CreateCartAsync(CreateCartRequest cartRequest);

    Task<CartModel> AddItemToCartAsync(AddItemToCartRequest request, string cartId, string itemId);
}