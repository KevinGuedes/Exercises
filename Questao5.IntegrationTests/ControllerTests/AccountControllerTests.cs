using FluentAssertions;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.IntegrationTests.Common;
using Questao5.TestCommon.TestData;
using System.Net;

namespace Questao5.IntegrationTests.ControllerTests;

public sealed class AccountControllerTests(ApplicationFactory factory)
    : BaseIntegrationTest(factory, "api/account")
{
    [Fact]
    public async Task GetAccountBalance_ShouldReturnSuccess_WhenRequestIsValid()
    {
        //Arrange
        var now = DateTime.UtcNow;
        var validAccount = AccountTestData.CreateActiveAccount();
        var defaultCreditTransfer = TransferTestData.CreateCreditTransferForAccount(validAccount.Id);
        var requestUrl = string.Format("{0}/Balance", validAccount.Id);

        //Act
        var (httpResponse, payload) = await GetAsync<GetAccountBalanceResponse>(requestUrl);

        //Assert
        Assert.True(httpResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        Assert.NotNull(payload);
        await VerifyResponsePayloadAsync(httpResponse);

        Assert.Equal(validAccount.HolderName, payload.AccountHolderName);
        Assert.Equal(validAccount.Number, payload.AccountNumber);
        //Precision is lost with SQLite
        Assert.Equal(Math.Round(defaultCreditTransfer.Value, 4), Math.Round(payload.Balance, 4));
        payload.BalanceCalculatedOnUtc.Should().BeAfter(now);
    }

    [Fact]
    public async Task GetAccountBalance_ShouldReturnBadRequest_WhenRequestHasInvalidAccount()
    {
        //Arrange
        var requestUrl = string.Format("{0}/Balance", Guid.Empty);

        //Act
        var (httpResponse, payload) = await GetAsync<GetAccountBalanceResponse>(requestUrl);

        //Assert
        Assert.False(httpResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(payload);
        await VerifyResponsePayloadAsync(httpResponse);
    }

    [Fact]
    public async Task GetAccountBalance_ShouldReturnBadRequest_WhenRequestHasInactiveAccount()
    {
        //Arrange
        var inactiveAccount = AccountTestData.CreateInactiveAccount();
        var requestUrl = string.Format("{0}/Balance", inactiveAccount.Id);

        //Act
        var (httpResponse, payload) = await GetAsync<GetAccountBalanceResponse>(requestUrl);

        //Assert
        Assert.False(httpResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(payload);
        await VerifyResponsePayloadAsync(httpResponse);
    }
}
