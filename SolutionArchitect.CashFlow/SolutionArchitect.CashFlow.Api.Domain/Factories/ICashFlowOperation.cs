namespace SolutionArchitect.CashFlow.Api.Domain.Factories;
public interface ICashFlowOperation
{
    decimal Execute(decimal currentBalance, decimal amount);
}
