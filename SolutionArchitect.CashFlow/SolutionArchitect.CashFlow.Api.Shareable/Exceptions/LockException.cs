namespace SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

public sealed class LockException : Exception
{
    public LockException() : base("Concurrent update detected.") { }
}
