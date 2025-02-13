using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Questao5.IntegrationTests.Common;

[Collection(nameof(ApplicationFactoryCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;

    protected BaseIntegrationTest(ApplicationFactory applicationFactory, string baseUrl)
    {
        _baseUrl = baseUrl;
        _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        _httpClient = applicationFactory.CreateClient();
    }

    protected async Task<(HttpResponseMessage, TResponse?)> GetAsync<TResponse>(
        string? requestUrl = null,
        bool overrideDefaultUrl = false)
    {
        var endpoint = overrideDefaultUrl ? requestUrl : GetEndpoint(requestUrl);
        var response = await _httpClient.GetAsync(endpoint);
        return await HandleHttpResponse<TResponse>(response);
    }

    protected async Task<(HttpResponseMessage, TResponse?)> PostAsync<TRequest, TResponse>(
        TRequest request,
        string? requestUrl = null)
    {
        var response = await _httpClient.PostAsJsonAsync(GetEndpoint(requestUrl), request);
        return await HandleHttpResponse<TResponse>(response);
    }

    protected static async Task VerifyResponsePayloadAsync(HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentLength is 0)
            return;

        var responseContent = await response.Content.ReadAsStringAsync();

        DerivePathInfo((_, projectDirectory, type, method) =>
        {
            return new PathInfo(
                Path.Combine(projectDirectory, "ResponseSnapshots", type.Name),
                type.Name,
                method.Name);
        });

        await VerifyJson(responseContent).UseStrictJson();
    }

    private async ValueTask<(HttpResponseMessage, TResponse?)> HandleHttpResponse<TResponse>(HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentLength is not 0)
        {
            var responseDataAsJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<TResponse>(responseDataAsJson, _jsonSerializerOptions);

            return (response, responseData);
        }

        return (response, default);
    }

    private string GetEndpoint(string? requestUrl)
        => requestUrl is null ? _baseUrl : $"{_baseUrl}/{requestUrl}";

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}