namespace SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
}

public sealed class RequestDataInvalidException(IEnumerable<string> errors) : DomainException("Invalid request data")
{
    public IReadOnlyCollection<string> Errors { get; } = [.. errors];
}

public sealed class NotFoundException(string message) : DomainException(message)
{
}

public sealed class AppException(string message) : DomainException(message)
{
}

public sealed class LockException(string message) : DomainException(message)
{
}

public sealed class InvalidOperationTypeException(string operation) : DomainException($"Invalid operation type: {operation}")
{
}
