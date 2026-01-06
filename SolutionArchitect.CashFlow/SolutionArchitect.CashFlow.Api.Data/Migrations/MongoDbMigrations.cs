using MongoDB.Driver;
using SolutionArchitect.CashFlow.Api.Shareable.Dtos;

namespace SolutionArchitect.CashFlow.Api.Data.Migrations;
public sealed class MongoDbMigrations(MongoDbContext context)
{
    public async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        var uniqueDateIndex = new CreateIndexModel<CashFlowDocument>(
            Builders<CashFlowDocument>.IndexKeys
                .Ascending(x => x.Date),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "ux_cashflow_date"
            });

        var dateVersionIndex = new CreateIndexModel<CashFlowDocument>(
            Builders<CashFlowDocument>.IndexKeys
                .Ascending(x => x.Date)
                .Ascending(x => x.Version),
            new CreateIndexOptions
            {
                Name = "ix_cashflow_date_version"
            });

        await context.CashFlow.Indexes.CreateManyAsync(
            [uniqueDateIndex, dateVersionIndex],
            cancellationToken);
    }
}
