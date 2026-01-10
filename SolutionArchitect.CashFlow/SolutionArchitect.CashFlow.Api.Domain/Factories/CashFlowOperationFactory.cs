using SolutionArchitect.CashFlow.Api.Domain.Services;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Domain.Factories;

public sealed class CashFlowOperationFactory
{
    public ICashFlowOperation Create(CashFlowOperationType operationType)
        => operationType switch
        {
            CashFlowOperationType.Credit => new CreditOperation(),
            CashFlowOperationType.Debit => new DebitOperation(),
            _ => throw new InvalidOperationTypeException(operationType.ToString())
        };
}
