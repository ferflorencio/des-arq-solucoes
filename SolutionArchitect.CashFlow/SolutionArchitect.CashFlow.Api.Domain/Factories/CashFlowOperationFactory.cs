using SolutionArchitect.CashFlow.Api.Domain.Services;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;

namespace SolutionArchitect.CashFlow.Api.Domain.Factories;

public sealed class CashFlowOperationFactory
{
    public ICashFlowOperation Create(CashFlowOperationType operationType)
        => operationType switch
        {
            CashFlowOperationType.Credit => new CreditOperation(),
            CashFlowOperationType.Debit => new DebitOperation(),
            _ => throw new NotSupportedException("Operation type not supported.")
        };
}
