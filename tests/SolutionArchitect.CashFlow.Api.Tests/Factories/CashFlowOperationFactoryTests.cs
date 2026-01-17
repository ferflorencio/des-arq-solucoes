using SolutionArchitect.CashFlow.Api.Application.Factories;
using SolutionArchitect.CashFlow.Api.Application.Services;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Tests.Factories;

public sealed class CashFlowOperationFactoryTests
{
    [Fact]
    public void Create_WithCredit_ReturnsCreditOperation()
    {
        // Arrange
        var factory = new CashFlowOperationFactory();

        // Act
        var result = factory.Create(CashFlowOperationType.Credit);

        // Assert
        Assert.IsType<CreditOperation>(result);
    }

    [Fact]
    public void Create_WithDebit_ReturnsDebitOperation()
    {
        // Arrange
        var factory = new CashFlowOperationFactory();

        // Act
        var result = factory.Create(CashFlowOperationType.Debit);

        // Assert
        Assert.IsType<DebitOperation>(result);
    }

    [Fact]
    public void Create_WithInvalidOperation_ThrowsInvalidOperationTypeException()
    {
        // Arrange
        var factory = new CashFlowOperationFactory();
        var invalidValue = (CashFlowOperationType)99;

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationTypeException>(() => factory.Create(invalidValue));
        Assert.Equal($"Invalid operation type: {invalidValue}", ex.Message);
    }
}