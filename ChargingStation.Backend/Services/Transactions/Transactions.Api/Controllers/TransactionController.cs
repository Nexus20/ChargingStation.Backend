using ChargingStation.Common.Models.General;
using ChargingStation.Common.Models.Transactions.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.Models.Requests;
using Transactions.Application.Services.Transactions;

namespace Transactions.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("getall")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedCollection<TransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody]GetTransactionsRequest request, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionService.GetAsync(request, cancellationToken);

        return Ok(transactions);
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionService.GetByIdAsync(id, cancellationToken);

        return Ok(transaction);
    }
}