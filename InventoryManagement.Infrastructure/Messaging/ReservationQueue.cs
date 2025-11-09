using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Infrastructure.Messaging
{
    public class ReservationQueue : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ReservationQueue> _log;
        private readonly string _queueName = "payment.order-created.v1";

        public ReservationQueue(
            ConnectionFactory factory,
            IServiceScopeFactory scopeFactory,
            IConfiguration cfg,
            ILogger<ReservationQueue> log)
        {
            _serviceScopeFactory = scopeFactory;
            _log = log;

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            var exchange = cfg.GetSection("MQServer")["Exchange"] ?? "flashsale.topic";
            var exchangeType = cfg.GetSection("MQServer")["ExchangeType"] ?? "topic";

            _channel.ExchangeDeclareAsync(exchange, exchangeType, durable: true).GetAwaiter().GetResult();

            _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                passive: false).GetAwaiter().GetResult();

            _channel.QueueBindAsync(
                queue: _queueName,
                exchange: exchange,
                routingKey: "Order.Created").GetAwaiter().GetResult();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += OnMessageReceive;

            _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _log.LogInformation("Inventory service is listening for Order.Created ...");
            return Task.CompletedTask;
        }
        
        private async Task OnMessageReceive(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                using var scope = _serviceScopeFactory.CreateScope();

                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                await reservationService.ProcessReservation(message);

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                _log.LogInformation("Processed Order.Created for product {ProductId}, qty {Qty}", "evt.ProductId", "evt.Quantity");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error processing Order.Created message");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        }

        public override void Dispose()
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
            base.Dispose();
        }
    }
}