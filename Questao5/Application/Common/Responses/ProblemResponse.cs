using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Questao5.Application.Common.Responses;

public class ProblemResponse(ProblemDetails problemDetails)
{
    public string Title { get; init; }
        = problemDetails.Title ?? ReasonPhrases.GetReasonPhrase(problemDetails.Status!.Value);
    public string Type { get; init; } = problemDetails.Type!;
    public int Status { get; init; } = problemDetails.Status!.Value;
    public string Detail { get; init; } = problemDetails.Detail!;
    public string Instance { get; init; } = problemDetails.Instance!;
}