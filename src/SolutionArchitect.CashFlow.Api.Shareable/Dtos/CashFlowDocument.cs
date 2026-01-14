using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SolutionArchitect.CashFlow.Api.Shareable.Dtos;

public sealed class CashFlowDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; init; }

    public decimal Balance { get; init; }

    public DateTime Date { get; init; }

    public int Version { get; init; }
}
