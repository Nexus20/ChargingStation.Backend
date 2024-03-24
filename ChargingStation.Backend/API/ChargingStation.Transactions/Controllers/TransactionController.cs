using ChargingStation.Common.Models.General;
using ChargingStation.Transactions.Models.Requests;
using ChargingStation.Transactions.Models.Responses;
using ChargingStation.Transactions.Services.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStation.Transactions.Controllers;

[ApiController]
[Route("[controller]")]
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