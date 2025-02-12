using Questao5.Application.Common.Errors;

namespace Questao5.Application.Errors;

public sealed class InternalServerError(string message = "An error occurred while processing the request")
    : BaseError(message)
{
}