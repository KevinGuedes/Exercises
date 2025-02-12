using FluentValidation.Results;

namespace Questao5.Application.Common.Errors;

public sealed class BadRequestError : BaseError
{
    public IEnumerable<(string PropertyName, string ErrorMessage, string ErrorCode)> ValidationErrors { get; init; }

    public BadRequestError(IEnumerable<ValidationFailure> validationErrors)
        : base("Invalid payload data, check validation errors for more details")
    {
        ValidationErrors = validationErrors
            .Select(error => (error.PropertyName, error.ErrorMessage, error.ErrorCode));
    }

    public BadRequestError(string propertyName, string errorMessage, string errorCode)
        : base("Invalid payload data, check validation errors for more details")
    {
        ValidationErrors = [(propertyName, errorMessage, errorCode)];
    }
}