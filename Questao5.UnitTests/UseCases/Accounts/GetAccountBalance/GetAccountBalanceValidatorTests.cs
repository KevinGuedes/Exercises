using Bogus;
using FluentValidation.TestHelper;
using NSubstitute;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.UnitTests.Extensions;

namespace Questao5.UnitTests.UseCases.Accounts.GetAccountBalance;

public sealed class GetAccountBalanceValidatorTest
{
    private readonly IAccountRepository _accountRepository;
    private readonly GetAccountBalanceValidator _validator;

    public GetAccountBalanceValidatorTest()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        _validator = new GetAccountBalanceValidator(_accountRepository);
    }

    [Fact]
    public async Task ShouldReturnValidResult_WhenAccountExistsAndIsActive()
    {
        // Arrange
        var query = new Faker<GetAccountBalanceQuery>()
            .UsePrivateConstructor()
            .RuleFor(query => query.AccountId, f => f.Random.Guid())
            .Generate();

        var account = new Faker<Account>()
            .UsePrivateConstructor()
            .RuleFor(account => account.Id, _ => query.AccountId)
            .RuleFor(account => account.Number, f => f.Random.Number(1000, 9999))
            .RuleFor(account => account.HolderName, f => f.Person.FullName)
            .RuleFor(account => account.IsActive, _ => true)
            .Generate();

        _accountRepository.ExistsByIdAsync(query.AccountId).Returns(true);
        _accountRepository.GetByIdAsync(query.AccountId).Returns(account);

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ShouldReturnInvalidAccountCode_WhenAccountDoesNotExists()
    {
        // Arrange
        var query = new Faker<GetAccountBalanceQuery>()
            .UsePrivateConstructor()
            .RuleFor(query => query.AccountId, f => f.Random.Guid())
            .Generate();

        _accountRepository.ExistsByIdAsync(query.AccountId).Returns(false);
        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(query => query.AccountId)
            .WithErrorCode("INVALID_ACCOUNT")
            .WithErrorMessage("The account does not exist.");
    }

    [Fact]
    public async Task ShouldReturnInactiveAccountCode_WhenAccountIsNotActive()
    {
        // Arrange
        var query = new Faker<GetAccountBalanceQuery>()
            .UsePrivateConstructor()
            .RuleFor(query => query.AccountId, f => f.Random.Guid())
            .Generate();

        var account = new Faker<Account>()
            .UsePrivateConstructor()
            .RuleFor(account => account.Id, _ => query.AccountId)
            .RuleFor(account => account.Number, f => f.Random.Number(1000, 9999))
            .RuleFor(account => account.HolderName, f => f.Person.FullName)
            .RuleFor(account => account.IsActive, _ => false)
            .Generate();

        _accountRepository.ExistsByIdAsync(query.AccountId).Returns(true);
        _accountRepository.GetByIdAsync(query.AccountId).Returns(account);

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(query => query.AccountId)
            .WithErrorCode("INACTIVE_ACCOUNT")
            .WithErrorMessage("The account is inactive.");
    }
}
