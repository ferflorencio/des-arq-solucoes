using SolutionArchitect.CashFlow.Api.Application.Factories;

namespace SolutionArchitect.CashFlow.Api.Application.Services;
public sealed class CreditOperation : ICashFlowOperation
{
    public decimal Execute(decimal currentBalance, decimal amount)
    {
        Validate(amount);
        return currentBalance + amount;
    }

    private static void Validate(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Credit amount must be greater than zero.");
    }
}