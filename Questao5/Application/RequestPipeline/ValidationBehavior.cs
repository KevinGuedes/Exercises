using FluentResults;
using FluentValidation;
using MediatR;
using Questao5.Application.Abstractions;
using Questao5.Application.Common.Errors;

namespace Questao5.Application.RequestPipeline;

public sealed class ValidationBehavior<TRequest, TResponse>(
    ILogger<ValidationBehavior<TRequest, TResponse>> logger,
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseAppRequest
    where TResponse : ResultBase, new()
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            _logger.LogWarning("No validators found for {RequestName}", request.GetType().Name);
            return await next();
        }

        var validationContext = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(_validators.Select(validators =>
            validators.ValidateAsync(validationContext, cancellationToken)));

        var validationErrors = validationResults
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationResult => validationResult is not null)
            .ToList();

        if (validationErrors.Count is 0)
            return await next();

        var badRequestResponse = new TResponse();
        var badRequestErrorResult = Result.Fail(new BadRequestError(validationErrors));
        badRequestResponse.Reasons.AddRange(badRequestErrorResult.Reasons);
        return badRequestResponse;
    }
}