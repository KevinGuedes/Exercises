using Bogus;
using FluentValidation.TestHelper;
using NSubstitute;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.TestCommon.Extensions;
using Questao5.TestCommon.TestData;

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
        var account = AccountTestData.CreateActiveAccount(false);
        var query = AccountTestData.CreateGetAccountBalanceQuery(account.Id, false);
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
        var query = AccountTestData.CreateGetAccountBalanceQueryForInvalidAccount();
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
        var account = AccountTestData.CreateInactiveAccount(false);
        var query = AccountTestData.CreateGetAccountBalanceQuery(account.Id, false);
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
