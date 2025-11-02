namespace InventoryManagement.Application.Dtos;

public class GetProductDto
{
    public string ProductId { get; set; }

    public string Name { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public int? SaleId { get; set; }

    public int AvailableStock { get; set; }
}