namespace Questao5.Application.Common.Errors;

public sealed class InternalServerError(string message = "An error occurred while processing the request")
    : BaseError(message)
{
}