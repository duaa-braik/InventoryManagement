using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : ControllerBase
{
    
}