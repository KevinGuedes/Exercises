namespace Questao5.Application.Common.Errors;

public sealed class EntityNotFoundError(string message) : BaseError(message)
{
}
