using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SolutionArchitect.CashFlow.Api.Shareable.Dtos;

namespace SolutionArchitect.CashFlow.Api.Data.Database;

public sealed class MongoDbContext(IMongoClient client, IOptions<MongoOptions> options)
{
    private readonly IMongoDatabase _database = client.GetDatabase(options.Value.Database);
    public IMongoCollection<CashFlowDocument> CashFlow => _database.GetCollection<CashFlowDocument>("cashflow");
}
