using MongoDB.Driver;
using SolutionArchitect.CashFlow.Api.Data.Mappers;
using SolutionArchitect.CashFlow.Api.Domain.Aggregates;
using SolutionArchitect.CashFlow.Api.Shareable.Dtos;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Data.Repository;

public sealed class CashFlowRepository(MongoDbContext context) : ICashFlowRepository
{
    private readonly MongoDbContext _context = context;

    public async Task<CashFlowAggregate?> GetByDateAsync(
        DateTime date,
        CancellationToken cancellationToken)
    {
        var document = await _context.CashFlow
            .Find(x => x.Date == date)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null
            ? null
            : CashFlowMapper.ToDomain(document);
    }

    public async Task TrySaveAsync(
        CashFlowAggregate aggregate,
        CancellationToken cancellationToken)
    {
        var filter = Builders<CashFlowDocument>.Filter.And(
            Builders<CashFlowDocument>.Filter.Eq(x => x.Date, aggregate.Date),
            Builders<CashFlowDocument>.Filter.Eq(x => x.Version, aggregate.Version)
        );

        var update = Builders<CashFlowDocument>.Update
            .Set(x => x.Balance, aggregate.Balance)
            .Inc(x => x.Version, 1);

        var result = await _context.CashFlow.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = aggregate.Version == 0 },
            cancellationToken);

        if (result.MatchedCount == 0 && aggregate.Version != 0)
        {
            throw new LockException();
        }
    }
}
