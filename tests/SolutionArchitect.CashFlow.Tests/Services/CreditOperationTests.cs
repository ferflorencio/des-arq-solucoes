using System;
using SolutionArchitect.CashFlow.Api.Application.Services;

namespace SolutionArchitect.CashFlow.Api.Tests.Services;

public class CreditOperationTests
{
    [Fact]
    public void Execute_WithPositiveAmount_ReturnsIncreasedBalance()
    {
        // Arrange
        var sut = new CreditOperation();
        decimal currentBalance = 100.50m;
        decimal amount = 49.50m;

        // Act
        var result = sut.Execute(currentBalance, amount);

        // Assert
        Assert.Equal(150.00m, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10.5)]
    public void Execute_WithNonPositiveAmount_ThrowsArgumentException(decimal invalidAmount)
    {
        // Arrange
        var sut = new CreditOperation();
        decimal currentBalance = 100m;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => sut.Execute(currentBalance, invalidAmount));
        Assert.Equal("Credit amount must be greater than zero.", ex.Message);
    }
}