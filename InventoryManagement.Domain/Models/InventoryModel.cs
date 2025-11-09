namespace InventoryManagement.Domain.Models;

public class InventoryModel
{
    public string Id { get; set; }

    public string ProductId { get; set; }

    public int AvailableQuantity { get; set; }

    public int ReservedQuantity { get; set; }
}