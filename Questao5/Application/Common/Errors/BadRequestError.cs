using FluentValidation.Results;

namespace Questao5.Application.Common.Errors;

public sealed class BadRequestError : BaseError
{
    public IEnumerable<(string PropertyName, string ErrorMessage)> ValidationErrors { get; init; }

    public BadRequestError(IEnumerable<ValidationFailure> validationErrors)
        : base("Invalid payload data, check validation errors for more details")
    {
        ValidationErrors = validationErrors
            .Select(error => (error.PropertyName, error.ErrorMessage));
    }

    public BadRequestError(string propertyName, string errorMessage)
        : base("Invalid payload data, check validation errors for more details")
    {
        ValidationErrors = new List<(string PropertyName, string ErrorMessage)> { (propertyName, errorMessage) };
    }
}