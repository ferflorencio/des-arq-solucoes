using SolutionArchitect.CashFlow.Api.Domain.Factories;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Domain.Services;

public sealed class DebitOperation : ICashFlowOperation
{
    public decimal Execute(decimal currentBalance, decimal amount)
    {
        Validate(currentBalance, amount);
        return currentBalance - amount;
    }

    private static void Validate(decimal currentBalance, decimal amount)
    {
        if (amount <= 0)
            throw new RequestDataInvalidException(["Debit amount must be greater than zero."]);

        if (amount > currentBalance)
            throw new RequestDataInvalidException(["Insufficient balance."]);
    }
}
