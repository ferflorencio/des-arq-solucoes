using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SolutionArchitect.CashFlow.Consolidate.Worker.Application.Cache;
using SolutionArchitect.CashFlow.Consolidate.Worker.Application.Messaging;
using System.Text;
using System.Text.Json;

namespace SolutionArchitect.CashFlow.Consolidate.Worker.Data.Messaging;

public sealed class CashFlowEventConsumer(
    IConnection connection,
    ICashFlowCache cache,
    ILogger<CashFlowEventConsumer> logger) : ICashFlowEventConsumer
{
    private const string ExchangeName = "cashflow.events";
    private const string QueueName = "cashflow.cache.worker";
    private IChannel? _channel;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await connection.CreateChannelAsync(null, cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: string.Empty,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message =
                    JsonSerializer.Deserialize<CashFlowUpdatedEvent>(json)!;

                await cache.SetAsync(
                    message.Date,
                    message.NewBalance,
                    cancellationToken);

                logger.LogInformation("Message saved on cache | Date: {Date} | Balance: {Balance}", message.Date, message.NewBalance);

                await _channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error to process CashFlow Message");
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        logger.LogInformation("CashFlowEventConsumer is listening messages...");
    }
}
