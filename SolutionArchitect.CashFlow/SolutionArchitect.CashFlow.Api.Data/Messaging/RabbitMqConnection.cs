using RabbitMQ.Client;

namespace SolutionArchitect.CashFlow.Api.Data.Messaging;

public sealed class RabbitMqConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnection(RabbitMqOptions options)
    {
        _factory = new ConnectionFactory
        {
            Uri = new Uri(options.ConnectionString)
        };
    }

    public async Task<IChannel> CreateChannelAsync(
        CancellationToken cancellationToken = default)
    {
        if (_connection is null || !_connection.IsOpen)
        {
            _connection = await _factory.CreateConnectionAsync(cancellationToken);
        }

        return await _connection.CreateChannelAsync(null, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}