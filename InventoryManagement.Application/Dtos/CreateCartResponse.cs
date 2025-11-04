namespace InventoryManagement.Application.Dtos;

public class CreateCartResponse
{
    public string CartId { get; set; }

    public string UserId { get; set; }

    public List<GetProductDto> Products { get; set; }
}