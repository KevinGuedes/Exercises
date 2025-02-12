using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Common.Responses;
using Questao5.Application.UseCases.Transfers.RegisterTransfer;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Presentation.Controllers;

[SwaggerTag("Transfers management")]
public class TransferController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    [SwaggerOperation(Summary = "Registers a new Transfer")]
    [SwaggerResponse(StatusCodes.Status201Created, "Transfer registered", typeof(RegisterTransferResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload", typeof(ValidationProblemResponse))]
    public async Task<IActionResult> RegisterTransfersAsync(
        [FromBody] [SwaggerRequestBody("Transfer's payload", Required = true)]
        RegisterTransferCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return ProcessResult(result, true);
    }
}
