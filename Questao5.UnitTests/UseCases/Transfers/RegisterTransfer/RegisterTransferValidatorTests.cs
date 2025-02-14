using FluentValidation.TestHelper;
using NSubstitute;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.TestCommon.TestData;

namespace Questao5.UnitTests.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferValidatorTests
{
    private readonly IAccountRepository _accountRepository;
    private readonly RegisterTransferValidator _validator;

    public RegisterTransferValidatorTests()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        _validator = new RegisterTransferValidator(_accountRepository);
    }

    [Fact]
    public async Task ShouldReturnValidResult_WhenCommandHasValidData()
    {
        //Arrange
        var account = AccountTestData.CreateActiveAccount(false);
        var command = TransferTestData.CreateRegisterTransferCommand(account.Id, false);

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
        var command = TransferTestData.CreateRegisterTransferCommandWithInvalidType();

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
        var command = TransferTestData.CreateRegisterTransferCommandWithInvalidValue();

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.Value)
            .WithErrorCode("INVALID_VALUE")
            .WithErrorMessage("The value must be greater than or equal to zero.");
    }

    [Fact]
    public async Task ShouldReturnInactiveAccountCode_WhenKeyIsInvalid()
    {
        //Arrange
        var account = AccountTestData.CreateInactiveAccount();
        var command = TransferTestData.CreateRegisterTransferCommandWithInvalidKey();

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.Key)
            .WithErrorCode("INVALID_KEY")
            .WithErrorMessage("The key must not be empty.");
    }

    [Fact]
    public async Task ShouldReturnInvalidAccountCode_WhenAccountDoesNotExist()
    {
        //Arrange
        var command = TransferTestData.CreateRegisterTransferCommandWithInvalidAccount();
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
        var account = AccountTestData.CreateInactiveAccount();
        var command = TransferTestData.CreateRegisterTransferCommand(account.Id, false);
        _accountRepository.ExistsByIdAsync(command.AccountId).Returns(true);
        _accountRepository.GetByIdAsync(command.AccountId).Returns(account);

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.AccountId)
            .WithErrorCode("INACTIVE_ACCOUNT")
            .WithErrorMessage("The account is inactive.");
    }

    [Fact]
    public async Task ShouldStopOnFirstError_WhenAccountThereAreMultipleErrorsInTheRequest()
    {
        //Arrange
        var command = TransferTestData.CreateRegisterTransferCommandWithMultipleInvalidFields();

        //Act
        var result = await _validator.TestValidateAsync(command);

        //Assert
        result.ShouldHaveValidationErrorFor(command => command.Type)
            .WithErrorCode("INVALID_TYPE")
            .WithErrorMessage("The type must be 'C' or 'D'.");

        await _accountRepository //CascadeMode.Stop avoids unecessary calls to database
            .DidNotReceive()
            .ExistsByIdAsync(Arg.Any<Guid>());

        await _accountRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>());
    }
}
