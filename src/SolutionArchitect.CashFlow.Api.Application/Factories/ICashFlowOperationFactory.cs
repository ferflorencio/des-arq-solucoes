using SolutionArchitect.CashFlow.Api.Shareable.Enums;

namespace SolutionArchitect.CashFlow.Api.Application.Factories
{
    public interface ICashFlowOperationFactory
    {
        ICashFlowOperation Create(CashFlowOperationType operationType);
    }
}
