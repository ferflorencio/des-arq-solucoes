using SolutionArchitect.CashFlow.Api.Application.Services;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Application.Factories;

public sealed class CashFlowOperationFactory : ICashFlowOperationFactory
{
    public ICashFlowOperation Create(CashFlowOperationType operationType)
        => operationType switch
        {
            CashFlowOperationType.Credit => new CreditOperation(),
            CashFlowOperationType.Debit => new DebitOperation(),
            _ => throw new InvalidOperationTypeException(operationType.ToString())
        };
}
