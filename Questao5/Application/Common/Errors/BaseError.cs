using FluentResults;

namespace Questao5.Application.Common.Errors;

public abstract class BaseError(string message) : Error(message)
{
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}