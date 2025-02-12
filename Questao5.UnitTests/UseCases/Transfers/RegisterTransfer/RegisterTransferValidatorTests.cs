using Bogus;
using FluentValidation.TestHelper;
using NSubstitute;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.UnitTests.Extensions;

namespace Questao5.UnitTests.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferValidatorTests
{
    private readonly IAccountRepository _accountRepository;
    private readonly RegisterTransferValidator _validator;
    private static readonly string[] _validTransferTypes = ["C", "D"];
    private static readonly string[] _invalidTransferTypes = ["A", "B"];

    public RegisterTransferValidatorTests()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        _validator = new RegisterTransferValidator(_accountRepository);
    }

    [Fact]
    public async Task ShouldReturnValidResult_WhenCommandHasValidData()
    {
        //Arrange
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

        _accountRepository.ExistsByIdAsync(command.AccountId).Returns(true);
        _accountRepository.GetByIdAsync(command.AccountId).Returns(account);

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ShouldReturnInvalidTypeCode_WhenTypeIsNotCOrD()
    {
        //Arrange
        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(_invalidTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

        _accountRepository.ExistsByIdAsync(command.AccountId).Returns(true);

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.Type)
            .WithErrorCode("INVALID_TYPE")
            .WithErrorMessage("The type must be 'C' or 'D'.");
    }

    [Fact]
    public async Task ShouldReturnInvalidValueCode_WhenValueIsLowerThanZero()
    {
        //Arrange
        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(-1000, -1))
            .RuleFor(command => command.Type, f => f.PickRandom(_validTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();
        _accountRepository.ExistsByIdAsync(command.AccountId).Returns(true);

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.Value)
            .WithErrorCode("INVALID_VALUE")
            .WithErrorMessage("The value must be greater than or equal to zero.");
    }

    [Fact]
    public async Task ShouldReturnInvalidAccountCode_WhenAccountDoesNotExist()
    {
        //Arrange
        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(_validTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

        _accountRepository.ExistsByIdAsync(command.AccountId).Returns(false);

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.AccountId)
            .WithErrorCode("INVALID_ACCOUNT")
            .WithErrorMessage("The account does not exist.");
    }

    [Fact]
    public async Task ShouldReturnInactiveAccountCode_WhenAccountIsNotActive()
    {
        //Arrange
        var command = new Faker<RegisterTransferCommand>()
            .UsePrivateConstructor()
            .RuleFor(command => command.AccountId, f => f.Random.Guid())
            .RuleFor(command => command.Key, f => f.Random.Guid())
            .RuleFor(command => command.Value, f => f.Random.Decimal(0, 1000))
            .RuleFor(command => command.Type, f => f.PickRandom(_validTransferTypes))
            .RuleFor(command => command.Date, f => DateOnly.FromDateTime(f.Date.Past()))
            .Generate();

        var account = new Faker<Account>()
            .UsePrivateConstructor()
            .RuleFor(account => account.Id, _ => command.AccountId)
            .RuleFor(account => account.Number, f => f.Random.Number(1000, 9999))
            .RuleFor(account => account.HolderName, f => f.Person.FullName)
            .RuleFor(account => account.IsActive, _ => false)
            .Generate();

        _accountRepository.ExistsByIdAsync(command.AccountId).Returns(true);
        _accountRepository.GetByIdAsync(command.AccountId).Returns(account);

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.AccountId)
            .WithErrorCode("INACTIVE_ACCOUNT")
            .WithErrorMessage("The account is inactive.");
    }
}
