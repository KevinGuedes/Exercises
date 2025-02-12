using Microsoft.AspNetCore.Mvc;

namespace Questao5.Application.Common.Responses;

public sealed class ValidationProblemResponse(ValidationProblemDetails validationProblemDetails)
{
    public string Title { get; init; } = validationProblemDetails.Title!;
    public string Type { get; init; } = validationProblemDetails.Type!;
    public int Status { get; init; } = validationProblemDetails.Status!.Value;
    public string Detail { get; init; } = validationProblemDetails.Detail!;
    public string Instance { get; init; } = validationProblemDetails.Instance!;
    public IReadOnlyDictionary<string, string[]> Errors { get; init; }
        = validationProblemDetails.Errors.AsReadOnly();
}