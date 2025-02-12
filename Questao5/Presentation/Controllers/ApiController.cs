using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Questao5.Application.Common.Errors;
using Questao5.Application.Common.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/problem+json")]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "Backend went rogue", typeof(ProblemResponse))]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected readonly ISender _sender = sender;

    protected ObjectResult ProcessResult<TResponse>(Result<TResponse> result, bool hasEntityBeenCreated = false)
        => result.IsFailed ? HandleFailureResult(result.Errors) : Ok(result.Value);

    protected ObjectResult HandleFailureResult(IEnumerable<IError> errors)
        => HandleFailureResult(errors.FirstOrDefault());

    protected ObjectResult HandleFailureResult(IError? error)
        => error switch
        {
            BadRequestError badRequest
                => BuildValidationProblemResponse(badRequest),
            InternalServerError internalServerError
                => BuildProblemResponse(StatusCodes.Status500InternalServerError, internalServerError),
            null
                => BuildProblemResponse(StatusCodes.Status500InternalServerError),
            _
                => BuildProblemResponse(StatusCodes.Status500InternalServerError)
        };

    private ObjectResult BuildValidationProblemResponse(BadRequestError badRequestError)
    {
        var modelStateDictionary = new ModelStateDictionary();

        badRequestError
            .ValidationErrors
            .ToList()
            .ForEach(error => modelStateDictionary.AddModelError(error.ErrorCode, error.ErrorMessage));

        var validationProblemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(
            HttpContext,
            modelStateDictionary,
            StatusCodes.Status400BadRequest,
            detail: "Invalid payload data, check the errors for more information",
            instance: HttpContext.Request.Path);

        return new(new ValidationProblemResponse(validationProblemDetails))
        {
            StatusCode = validationProblemDetails.Status
        };
    }

    private ObjectResult BuildProblemResponse(int statusCode, IError error)
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: statusCode,
            detail: error.Message,
            instance: HttpContext.Request.Path);

        return new(new ProblemResponse(problemDetails))
        {
            StatusCode = problemDetails!.Status
        };
    }

    private ObjectResult BuildProblemResponse(int statusCode)
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: statusCode,
            detail: "MyFinance API went rogue! Sorry!",
            instance: HttpContext.Request.Path);

        return new(new ProblemResponse(problemDetails))
        {
            StatusCode = problemDetails!.Status
        };
    }
}