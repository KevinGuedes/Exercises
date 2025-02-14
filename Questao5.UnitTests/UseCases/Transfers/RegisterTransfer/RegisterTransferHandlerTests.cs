using NSubstitute;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Domain.Interfaces.Services;
using Questao5.TestCommon.TestData;

namespace Questao5.UnitTests.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferHandlerTests
{
    private readonly RegisterTransferHandler _handler;
    private readonly ITransferRepository _transferRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly ISerializerService _serializerService;

    public RegisterTransferHandlerTests()
    {
        _transferRepository = Substitute.For<ITransferRepository>();
        _idempotencyRepository = Substitute.For<IIdempotencyRepository>();
        _serializerService = Substitute.For<ISerializerService>();

        _handler = new RegisterTransferHandler(
            _transferRepository,
            _idempotencyRepository,
            _serializerService);
    }

    [Fact]
    public async Task ShouldReturnValidResult_WhenTransferIsRegistered()
    {
        // Arrange
        var account = AccountTestData.CreateActiveAccount(false);
        var command = TransferTestData.CreateRegisterTransferCommand(account.Id, false);
        var transfer = TransferTestData.CreateCreditTransferForAccount(account.Id, false);

        _idempotencyRepository.GetByKeyAsync(command.Key).Returns((Idempotency)null!);
        _transferRepository.InsertAsync(Arg.Any<Transfer>()).Returns(Task.CompletedTask);
        _serializerService.Serialize(command).Returns("serializedCommand");
        _serializerService.Serialize(Arg.Any<RegisterTransferResponse>()).Returns("serializedResponse");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var response = result.Value;

        _serializerService
            .Received(1)
            .Serialize(command);

        _serializerService
            .Received(1)
            .Serialize(Arg.Any<RegisterTransferResponse>());

        await _transferRepository
            .Received(1)
            .InsertAsync(Arg.Is<Transfer>(t =>
                t.AccountId == command.AccountId &&
                t.Type == command.Type &&
                t.Value == command.Value));
    }

    [Fact]
    public async Task ShouldReturnResultStoredInIdempotency_WhenHandlingCommandWithExistingKey()
    {
        // Arrange
        var command = TransferTestData.CreateRegisterTransferCommand(null, false);
        var response = new RegisterTransferResponse(Guid.NewGuid());

        var idempotency = new Idempotency(
            command.Key,
            "serializedCommand",
            "serializedResponse");

        _idempotencyRepository.GetByKeyAsync(command.Key).Returns(idempotency);
        _serializerService.Deserialize<RegisterTransferResponse>(idempotency.Result).Returns(response);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        await _idempotencyRepository
            .Received(requiredNumberOfCalls: 1)
            .GetByKeyAsync(Arg.Is(command.Key));

        _serializerService
            .Received(1)
            .Deserialize<RegisterTransferResponse>(idempotency.Result);
    }
}
