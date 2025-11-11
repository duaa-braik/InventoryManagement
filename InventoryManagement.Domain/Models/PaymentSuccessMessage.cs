namespace InventoryManagement.Domain.Models;

public class PaymentSuccessMessage
{
    public string Id { get; set; }

    public string OrderId { get; set; }

    public string UserId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; }

    public string Status { get; set; }

    public string CorrelationId { get; set; }
}