using Bogus;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.Domain.Entities;
using Questao5.TestCommon.Extensions;

namespace Questao5.TestCommon.TestData;

public static class TransferTestData
{
    public static readonly string[] ValidTransferTypes = ["C", "D"];
    public static readonly string[] InvalidTransferTypes = ["A", "B"];

    public static RegisterTransferCommand CreateRegisterTransferCommand(
        Guid? accountId,
        bool useDefaultSeed = true)
    {
        accountId ??= Guid.NewGuid();

        var faker = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, accountId)
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .RuleFor(command => command.Type, f => f.PickRandom(ValidTransferTypes))
            .RuleFor(command => command.Value, f => f.Random.Decimal(1, 1000));

        if (useDefaultSeed)
            faker.UseSeed(4);

        return faker.Generate();
    }

    public static RegisterTransferCommand CreateRegisterTransferCommandWithInvalidType()
        => new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(InvalidTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

    public static RegisterTransferCommand CreateRegisterTransferCommandWithInvalidValue()
        => new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(-1000, -1))
            .RuleFor(command => command.Type, f => f.PickRandom(ValidTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

    public static RegisterTransferCommand CreateRegisterTransferCommandWithInvalidAccount()
        => new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, Guid.Empty)
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(ValidTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

    public static Transfer CreateCreditTransferForAccount(
        Guid accountId,
        bool useDefaultSeed = true)
    {
        var faker = new Faker<Transfer>()
            .UsePrivateConstructor()
            .RuleFor(transfer => transfer.Id, f => f.Random.Guid())
            .RuleFor(transfer => transfer.AccountId, accountId)
            .RuleFor(transfer => transfer.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .RuleFor(transfer => transfer.Type, _ => "C")
            .RuleFor(transfer => transfer.Value, f => f.Random.Decimal(1, 1000));

        if (useDefaultSeed)
            faker.UseSeed(3);

        return faker.Generate();
    }
}
