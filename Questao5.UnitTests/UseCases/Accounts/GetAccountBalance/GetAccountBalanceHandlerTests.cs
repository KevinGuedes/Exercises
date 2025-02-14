using NSubstitute;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.TestCommon.TestData;

namespace Questao5.UnitTests.UseCases.Accounts.GetAccountBalance;

public sealed class GetAccountBalanceHandlerTests
{
    private readonly GetAccountBalanceHandler _handler;
    private readonly IAccountRepository _accountRepository;

    public GetAccountBalanceHandlerTests()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        _handler = new GetAccountBalanceHandler(_accountRepository);
    }

    [Fact]
    public async Task ShouldReturnValidResponse_WhenCommandIsHandled()
    {
        //Arrange
        var expectedBalance = 1000;
        var account = AccountTestData.CreateActiveAccount(false);
        var query = AccountTestData.CreateGetAccountBalanceQuery(account.Id);

        _accountRepository.GetByIdAsync(query.AccountId).Returns(account);
        _accountRepository.GetBalanceAsync(query.AccountId).Returns(expectedBalance);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        var response = result.Value;

        Assert.Equal(account.Number, response.AccountNumber);
        Assert.Equal(account.HolderName, response.AccountHolderName);
        Assert.Equal(expectedBalance, response.Balance);
    }
}
