using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SolutionArchitect.CashFlow.Api.Application.Aggregates;
using SolutionArchitect.CashFlow.Api.Application.Factories;
using SolutionArchitect.CashFlow.Api.Application.Messaging;
using SolutionArchitect.CashFlow.Api.Application.Handlers;
using SolutionArchitect.CashFlow.Api.Application.Config;
using SolutionArchitect.CashFlow.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;

namespace SolutionArchitect.CashFlow.Api.Tests.Handlers
{
    public class ExecuteCashFlowOperationHandlerTests
    {
        [Fact]
        public async Task Handle_WithExistingAggregate_AppliesOperation_SavesAndPublishes()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var previousBalance = 100m;
            var amount = 50m;
            var expectedNewBalance = previousBalance + amount;

            var aggregate = CashFlowAggregate.Create(today);
            aggregate.Apply(previousBalance);

            var repositoryMock = new Mock<ICashFlowRepository>();
            repositoryMock
                .Setup(r => r.GetByDateAsync(It.Is<DateTime>(d => d.Date == today), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aggregate);

            repositoryMock
                .Setup(r => r.TrySaveAsync(It.IsAny<CashFlowAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var publisherMock = new Mock<IEventPublisher>();
            publisherMock
                .Setup(p => p.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var operationMock = new Mock<ICashFlowOperation>();
            operationMock.Setup(o => o.Execute(previousBalance, amount)).Returns(expectedNewBalance);

            var factoryMock = new Mock<ICashFlowOperationFactory>();
            factoryMock.Setup(f => f.Create(It.IsAny<CashFlowOperationType>())).Returns(operationMock.Object);

                // Mock executor: forward the delegate execution as-is
            var executorMock = new Mock<IResiliencePipelineExecutor>();
            executorMock
                .Setup(p => p.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ExecuteCashFlowOperationResponse>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<ExecuteCashFlowOperationResponse>> action, CancellationToken ct) => action(ct));

            var handler = new ExecuteCashFlowOperationHandler(
                factoryMock.Object,
                repositoryMock.Object,
                publisherMock.Object,
                executorMock.Object);

            var request = new ExecuteCashFlowOperationRequest(CashFlowOperationType.Credit, amount);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(previousBalance, response.PreviousBalance);
            Assert.Equal(expectedNewBalance, response.NewBalance);
            Assert.Equal(CashFlowOperationType.Credit.ToString(), response.Operation);

            repositoryMock.Verify(r => r.TrySaveAsync(It.Is<CashFlowAggregate>(a => a.Date == today && a.Balance == expectedNewBalance), It.IsAny<CancellationToken>()), Times.Once);
            publisherMock.Verify(p => p.PublishAsync(It.Is<object>(e =>
                e.GetType().Name.Contains("CashFlowUpdatedEvent") &&
                (decimal)e.GetType().GetProperty("NewBalance")!.GetValue(e)! == expectedNewBalance), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNoAggregate_CreatesAggregate_AppliesOperation_SavesAndPublishes()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var previousBalance = 0m;
            var amount = 20m;
            var expectedNewBalance = previousBalance - amount; // exemplo: operação de débito

            var repositoryMock = new Mock<ICashFlowRepository>();
            repositoryMock
                .Setup(r => r.GetByDateAsync(It.Is<DateTime>(d => d.Date == today), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CashFlowAggregate?)null);

            repositoryMock
                .Setup(r => r.TrySaveAsync(It.IsAny<CashFlowAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var publisherMock = new Mock<IEventPublisher>();
            publisherMock
                .Setup(p => p.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var operationMock = new Mock<ICashFlowOperation>();
            operationMock.Setup(o => o.Execute(previousBalance, amount)).Returns(expectedNewBalance);

            var factoryMock = new Mock<ICashFlowOperationFactory>();
            factoryMock.Setup(f => f.Create(It.IsAny<CashFlowOperationType>())).Returns(operationMock.Object);

            // Mock executor: forward the delegate execution as-is
            var executorMock = new Mock<IResiliencePipelineExecutor>();
            executorMock
                .Setup(p => p.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ExecuteCashFlowOperationResponse>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<CancellationToken, Task<ExecuteCashFlowOperationResponse>> action, CancellationToken ct) => action(ct));

            var handler = new ExecuteCashFlowOperationHandler(
                factoryMock.Object,
                repositoryMock.Object,
                publisherMock.Object,
                executorMock.Object);

            var request = new ExecuteCashFlowOperationRequest(CashFlowOperationType.Debit, amount);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(previousBalance, response.PreviousBalance);
            Assert.Equal(expectedNewBalance, response.NewBalance);
            Assert.Equal(CashFlowOperationType.Debit.ToString(), response.Operation);

            repositoryMock.Verify(r => r.TrySaveAsync(It.Is<CashFlowAggregate>(a => a.Date == today && a.Balance == expectedNewBalance), It.IsAny<CancellationToken>()), Times.Once);
            publisherMock.Verify(p => p.PublishAsync(It.Is<object>(e =>
                e.GetType().Name.Contains("CashFlowUpdatedEvent") &&
                (decimal)e.GetType().GetProperty("NewBalance")!.GetValue(e)! == expectedNewBalance), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}