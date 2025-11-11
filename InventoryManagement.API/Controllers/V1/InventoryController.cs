using Asp.Versioning;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IInventoryService _inventoryService;

        public InventoryController(
            IReservationService reservationService,
            IInventoryService inventoryService)
        {
            _reservationService = reservationService;
            _inventoryService = inventoryService;
        }

        public sealed record ApplyOrderRequest(Guid OrderId);

        // will be called right after ORDER CREATION (to update Redis cache)
        // POST /api/v1/inventory/cache/apply-order
        [HttpPost("cache/apply-order")]
        public async Task<IActionResult> ApplyOrderToCache([FromBody] ApplyOrderRequest req)
        {
            await _reservationService.ProcessReservationForOrderAsync(req.OrderId);
            return Ok(new { message = "Cache updated from order", orderId = req.OrderId });
        }

        // will be called right after PAYMENT SUCCESS (to update DB)
        // POST /api/v1/inventory/db/apply-order
        [HttpPost("db/apply-order")]
        public async Task<IActionResult> ApplyOrderToDb([FromBody] ApplyOrderRequest req)
        {
            await _inventoryService.UpdateInventoryForOrderAsync(req.OrderId);
            return Ok(new { message = "Inventory updated from order", orderId = req.OrderId });
        }
    }
}
