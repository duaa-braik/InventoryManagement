using Asp.Versioning;
using InventoryManagement.Application.Dtos;
using InventoryManagement.Application.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/carts")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }
    
    [HttpPost]
    public async Task<ActionResult<CreateCartResponse>> CreateCart(CreateCartRequest cartRequest)
    {
        var createdCart = await _cartService.CreateCartAsync(cartRequest);   
        
        return Ok(createdCart.Adapt<CreateCartResponse>());
    }
}