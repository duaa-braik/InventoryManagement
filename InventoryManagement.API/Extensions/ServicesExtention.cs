using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure.Repositories;
using StackExchange.Redis;

namespace InventoryManagement.API.Extensions;

public static class ServicesExtention
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer
            .Connect(configuration["Redis"]!));
        services.AddScoped<IInventoryCacheService, InventoryCacheService>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryService, InventoryService>();
        return services;
    }
}