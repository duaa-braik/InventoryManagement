namespace InventoryManagement.Domain.Models;

public class CartModel
{
    public string CartId { get; set; }

    public string UserId { get; set; }

    public List<CartItemModel> CartItems { get; set; } = [];
}