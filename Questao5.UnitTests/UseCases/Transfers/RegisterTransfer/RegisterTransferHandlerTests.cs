using Bogus;
using NSubstitute;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Domain.Interfaces.Services;
using Questao5.UnitTests.Extensions;

namespace Questao5.UnitTests.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferHandlerTests
{
    private readonly RegisterTransferHandler _handler;
    private readonly ITransferRepository _transferRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly ISerializerService _serializerService;
    private static readonly string[] _validTransferTypes = ["C", "D"];

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
        var account = new Faker<Account>()
            .UsePrivateConstructor()
            .RuleFor(account => account.Id, f => f.Random.Guid())
            .RuleFor(account => account.Number, f => f.Random.Number(1000, 9999))
            .RuleFor(account => account.HolderName, f => f.Person.FullName)
            .RuleFor(account => account.IsActive, _ => true)
            .Generate();

        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, _ => account.Id)
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(_validTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

        var transfer = new Transfer(
            command.AccountId,
            command.Date,
            command.Type,
            command.Value);

        _idempotencyRepository.GetByKeyAsync(command.Key).Returns((Idempotency)null!);
        _transferRepository.InsertAsync(Arg.Any<Transfer>()).Returns(Task.CompletedTask);
        _serializerService.Serialize(command).Returns("serializedCommand");
        _serializerService.Serialize(Arg.Any<RegisterTransferResponse>()).Returns("serializedResponse");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var response = result.Value;

        Assert.Equal(transfer.AccountId, response.AccountId);
        Assert.Equal(transfer.Value, response.Value);
        Assert.Equal(transfer.Type, response.Type);

        _serializerService
            .Received(1)
            .Serialize(command);

        _serializerService
            .Received(1)
            .Serialize(Arg.Is<RegisterTransferResponse>(r =>
                r.AccountId == transfer.AccountId &&
                r.Value == transfer.Value &&
                r.Type == transfer.Type));

        await _transferRepository
            .Received(1)
            .InsertAsync(Arg.Is<Transfer>(t =>
                t.AccountId == command.AccountId &&
                t.Date == command.Date &&
                t.Type == command.Type &&
                t.Value == command.Value));
    }

    [Fact]
    public async Task ShouldReturnResultStoredInIdempotency_WhenHandlingCommandWithExistingKey()
    {
        // Arrange
        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(_validTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

        var response = new RegisterTransferResponse(
            Guid.NewGuid(),
            command.AccountId,
            command.Value,
            command.Type);

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

        Assert.Equal(command.AccountId, response.AccountId);
        Assert.Equal(command.Value, response.Value);
        Assert.Equal(command.Type, response.Type);

        await _idempotencyRepository
            .Received(requiredNumberOfCalls: 1)
            .GetByKeyAsync(Arg.Is(command.Key));

        _serializerService
            .Received(1)
            .Deserialize<RegisterTransferResponse>(idempotency.Result);
    }
}
