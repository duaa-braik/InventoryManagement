using System.Text;
using InventoryManagement.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryManagement.Infrastructure.Messaging;

public class ReservationQueue : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReservationQueue(ConnectionFactory factory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory; 
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(queue: "reservation-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var reservationService = scope
                .ServiceProvider
                .GetRequiredService<IReservationService>();

            await reservationService.ProcessReservation(message);
        };

        _channel.BasicConsumeAsync(queue: "reservation-created", autoAck: true, consumer: consumer,
            cancellationToken: stoppingToken);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.CloseAsync();
        _connection.CloseAsync();
        base.Dispose();
    }
}