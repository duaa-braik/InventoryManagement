namespace InventoryManagement.Application.Dtos;

public class CartItemDto
{
    public string ProductId { get; set; }

    public string Name { get; set; }

    public int LineItemId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
    
    public int SaleId { get; set; }

    public string ImageUrl { get; set; }
}