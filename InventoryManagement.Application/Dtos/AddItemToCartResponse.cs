namespace InventoryManagement.Application.Dtos;

public class AddItemToCartResponse
{
    public string CartId { get; set; }

    public string UserId { get; set; }

    public List<CartItemDto> CartItems { get; set; } = [];
}