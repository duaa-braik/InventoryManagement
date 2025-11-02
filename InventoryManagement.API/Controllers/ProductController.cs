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
    public ActionResult<List<GetProductDto>> GetProducts(int page, int pageSize)
    {
        var products = new List<GetProductDto>();
        var thread = new Thread(() =>
        {
            products = _inventoryService.GetProducts(page, pageSize).Result.Adapt<List<GetProductDto>>(); 
        });
        thread.Start();
        thread.Join();
        
        return Ok(products);
    }
}