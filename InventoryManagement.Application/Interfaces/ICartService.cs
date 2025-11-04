using InventoryManagement.Application.Dtos;
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Application.Interfaces;

public interface ICartService
{
    Task<CartModel> CreateCartAsync(CreateCartRequest cartRequest);
}