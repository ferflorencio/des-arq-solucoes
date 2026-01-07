using RabbitMQ.Client;
using SolutionArchitect.CashFlow.Api.Domain.Messaging;
using System.Text;
using System.Text.Json;

namespace SolutionArchitect.CashFlow.Api.Data.Messaging
{
    public sealed class RabbitMqEventPublisher(RabbitMqConnection connection, RabbitMqOptions options) : IEventPublisher
    {
        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class
        {
            await using var channel = await connection.CreateChannelAsync(cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: options.Exchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var payload = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(payload);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: options.Exchange,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}
