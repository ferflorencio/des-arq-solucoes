using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Domain;
using SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Domain.Messaging;
using System.Text;
using System.Text.Json;

namespace SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Data.Messaging;

public sealed class CashFlowEventConsumer(
    IConnection connection,
    ICashFlowCache cache,
    ILogger<CashFlowEventConsumer> logger)
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

                logger.LogInformation(
                    "Mensagem salva no Redis | Date: {Date} | Balance: {Balance}",
                    message.Date,
                    message.NewBalance);

                await _channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem do CashFlow");
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        logger.LogInformation("CashFlowEventConsumer está escutando mensagens...");
    }
}
