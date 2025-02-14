using FluentValidation.Results;

namespace Questao5.Application.Common.Errors;

public sealed class BadRequestError : BaseError
{
    public IEnumerable<(string ErrorMessage, string ErrorCode)> ValidationErrors { get; init; }

    public BadRequestError(IEnumerable<ValidationFailure> validationErrors)
        : base("Invalid payload data, check validation errors for more details")
    {
        ValidationErrors = validationErrors
            .Select(error => (error.ErrorMessage, error.ErrorCode));
    }

    public BadRequestError(string errorMessage, string errorCode)
        : base("Invalid payload data, check validation errors for more details")
    {
        ValidationErrors = [(errorMessage, errorCode)];
    }
}