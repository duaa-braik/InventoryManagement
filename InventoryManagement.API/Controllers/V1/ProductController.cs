using Asp.Versioning;
using InventoryManagement.Application.Dtos;
using InventoryManagement.Application.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    
    public ProductController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GetProductDto>>> GetProducts(int page, int pageSize)
    {
        var products = await _inventoryService.GetProducts(page, pageSize);
        return Ok(products.Adapt<List<GetProductDto>>());
    }
}