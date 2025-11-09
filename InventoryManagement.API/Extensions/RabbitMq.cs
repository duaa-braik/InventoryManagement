using InventoryManagement.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace InventoryManagement.API.Extensions;

public static class RabbitMq
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<ReservationQueue>();
        
        services.AddSingleton(
            new ConnectionFactory()
            {
                HostName = configuration["MQServer:HostName"]!,
                UserName = configuration["MQServer:Username"]!,
                Password = configuration["MQServer:Password"]!,
                VirtualHost = configuration["MQServer:VirtualHost"]!,
                Port = int.TryParse(configuration["MQServer:Port"], out var port) ? port : 0,
            });
            
        return services;
    }
}