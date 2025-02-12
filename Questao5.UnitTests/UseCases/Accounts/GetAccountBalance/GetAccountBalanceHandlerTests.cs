using Bogus;
using NSubstitute;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.UnitTests.Extensions;

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
