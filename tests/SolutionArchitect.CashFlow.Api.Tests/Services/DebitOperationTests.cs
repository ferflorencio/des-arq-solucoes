using SolutionArchitect.CashFlow.Api.Application.Services;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Tests.Services;

public class DebitOperationTests
{
    [Fact]
    public void Execute_WithPositiveAmount_ReturnsDecreasedBalance()
    {
        // Arrange
        var sut = new DebitOperation();
        decimal currentBalance = 200.75m;
        decimal amount = 50.25m;

        // Act
        var result = sut.Execute(currentBalance, amount);

        // Assert
        Assert.Equal(150.50m, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100.5)]
    public void Execute_WithNonPositiveAmount_ThrowsRequestDataInvalidException(decimal invalidAmount)
    {
        // Arrange
        var sut = new DebitOperation();
        decimal currentBalance = 100m;

        // Act & Assert
        var ex = Assert.Throws<RequestDataInvalidException>(() => sut.Execute(currentBalance, invalidAmount));
        Assert.NotNull(ex.Errors);
        Assert.Contains("Debit amount must be greater than zero.", ex.Errors);
    }
}