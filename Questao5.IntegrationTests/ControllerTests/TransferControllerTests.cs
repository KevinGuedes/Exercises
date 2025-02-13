using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Questao5.IntegrationTests.Common;
using Questao5.TestCommon.TestData;
using System.Net;

namespace Questao5.IntegrationTests.ControllerTests;

public sealed class TransferControllerTests(ApplicationFactory factory)
    : BaseIntegrationTest(factory, "api/transfer")
{
    [Fact]
    public async Task RegisterTransfer_ShouldReturnSuccess_WhenRequestIsValid()
    {
        //Arrange
        var validAccount = AccountTestData.CreateActiveAccount();
        var request = TransferTestData.CreateRegisterTransferCommand(validAccount.Id);

        //Act
        var (httpResponse, payload) = await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);

        //Assert
        Assert.True(httpResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        Assert.NotNull(payload);
        Assert.NotEqual(payload.Id, Guid.Empty);
        await VerifyResponsePayloadAsync(httpResponse);
    }

    [Fact]
    public async Task RegisterTransfer_ShouldReturnBadRequest_WhenRequestHasInvalidType()
    {
        //Arrange
        var request = TransferTestData.CreateRegisterTransferCommandWithInvalidType();

        //Act
        var (httpResponse, payload) = await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);

        //Assert
        Assert.False(httpResponse.IsSuccessStatusCode);
        await VerifyResponsePayloadAsync(httpResponse);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task RegisterTransfer_ShouldReturnBadRequest_WhenRequestHasInvalidValue()
    {
        //Arrange
        var request = TransferTestData.CreateRegisterTransferCommandWithInvalidValue();

        //Act
        var (httpResponse, payload) = await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);

        //Assert
        Assert.False(httpResponse.IsSuccessStatusCode);
        await VerifyResponsePayloadAsync(httpResponse);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task RegisterTransfer_ShouldReturnBadRequest_WhenRequestHasInactiveAccount()
    {
        //Arrange
        var account = AccountTestData.CreateInactiveAccount();
        var request = TransferTestData.CreateRegisterTransferCommand(account.Id);

        //Act
        var (httpResponse, payload) = await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);

        //Assert
        Assert.False(httpResponse.IsSuccessStatusCode);
        await VerifyResponsePayloadAsync(httpResponse);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task RegisterTransfer_ShouldReturnBadRequest_WhenRequestHasInvalidAccount()
    {
        //Arrange
        var request = TransferTestData.CreateRegisterTransferCommandWithInvalidAccount();

        //Act
        var (httpResponse, payload) = await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);

        //Assert
        Assert.False(httpResponse.IsSuccessStatusCode);
        await VerifyResponsePayloadAsync(httpResponse);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task RegisterTransfer_ShouldBeIdempotent_WhenSameRequestIsSentTwice()
    {
        //Arrange
        var account = AccountTestData.CreateActiveAccount();
        var request = TransferTestData.CreateRegisterTransferCommand(account.Id);
        await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);
        var requestUrl = string.Format("api/Account/{0}/Balance", account.Id);
        var (_, previousBalanceData) = await GetAsync<GetAccountBalanceResponse>(requestUrl, true);

        //Act
        await PostAsync<RegisterTransferCommand, RegisterTransferResponse>(request);
        var (response, currentBalanceData) = await GetAsync<GetAccountBalanceResponse>(requestUrl, true);

        //Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(currentBalanceData);
        //Precision is lost with SQLite
        Assert.Equal(Math.Round(previousBalanceData!.Balance, 4), Math.Round(currentBalanceData.Balance, 4));
    }
}
