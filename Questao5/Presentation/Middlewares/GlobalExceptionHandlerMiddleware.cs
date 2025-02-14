using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Questao5.Application.Common.Responses;

namespace Questao5.Presentation.Middlewares;

public class GlobalExceptionHandlerMiddleware(
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    ProblemDetailsFactory problemDetailsFactory)
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var internalServerErrorProblemResponse = BuildInternalServerErrorProblemResponse(httpContext);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            value: internalServerErrorProblemResponse.Value,
            type: internalServerErrorProblemResponse.Value!.GetType(),
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private ObjectResult BuildInternalServerErrorProblemResponse(HttpContext httpContext)
    {
        var problemDetails = _problemDetailsFactory.CreateProblemDetails(
            httpContext,
            instance: httpContext.Request.Path,
            statusCode: StatusCodes.Status500InternalServerError,
            detail: "Financial API went rogue. Sorry!");

        return new(new ProblemResponse(problemDetails))
        {
            StatusCode = problemDetails!.Status
        };
    }
}