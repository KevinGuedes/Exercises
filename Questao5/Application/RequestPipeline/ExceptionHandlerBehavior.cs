using FluentResults;
using MediatR.Pipeline;
using Questao5.Application.Abstractions;
using Questao5.Application.Common.Errors;

namespace Questao5.Application.RequestPipeline;

public sealed class ExceptionHandlerBehavior<TRequest, TResponse, TException>(ILogger<ExceptionHandlerBehavior<TRequest, TResponse, TException>> logger)
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IBaseAppRequest
    where TResponse : ResultBase, new()
    where TException : Exception
{
    private readonly ILogger<ExceptionHandlerBehavior<TRequest, TResponse, TException>> _logger = logger;

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        var errorResult = BuildInternalServerErrorResult(exception, request.GetType().Name);

        var response = new TResponse();
        response.Reasons.AddRange(errorResult.Reasons);
        state.SetHandled(response);

        return Task.CompletedTask;
    }

    private Result BuildInternalServerErrorResult(TException exception, string requestName)
    {
        _logger.LogError(exception, "An error occurred while processing {RequestName}", requestName);
        var internalServerError = new InternalServerError().CausedBy(exception);
        return Result.Fail(internalServerError);
    }
}