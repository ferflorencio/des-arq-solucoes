namespace SolutionArchitect.CashFlow.Api.Application.Factories;
public interface ICashFlowOperation
{
    decimal Execute(decimal currentBalance, decimal amount);
}
