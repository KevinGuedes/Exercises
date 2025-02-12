using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Common.Responses;
using Questao5.Application.UseCases.Accounts.GetAccountBalance;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Presentation.Controllers;

[SwaggerTag("Account management")]
public class AccountController(ISender sender) : ApiController(sender)
{
    [HttpGet("{id:guid}/Balance")]
    [SwaggerOperation(Summary = "Gets the Account Balanca by it's Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "The Account data", typeof(GetAccountBalanceResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters", typeof(ValidationProblemResponse))]
    public async Task<IActionResult> GetAccountBalanceByIdAsync(
       [FromRoute][SwaggerParameter("The Account Id", Required = true)] Guid id,
       CancellationToken cancellationToken)
    {
        var query = new GetAccountBalanceQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return ProcessResult(result);
    }
}
