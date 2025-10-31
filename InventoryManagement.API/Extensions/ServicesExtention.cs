using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure.Repositories;

namespace InventoryManagement.API.Extensions;

public static class ServicesExtention
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryCacheService, InventoryCacheService>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        return services;
    }
}