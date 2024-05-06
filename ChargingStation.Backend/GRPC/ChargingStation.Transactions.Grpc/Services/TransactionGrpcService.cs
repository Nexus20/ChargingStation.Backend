using ChargingStation.Transactions.Grpc.Extensions;
using ChargingStation.Transactions.Services.Transactions;
using Grpc.Core;
using Transactions.Grpc;

namespace ChargingStation.Transactions.Grpc.Services;

public class TransactionGrpcService : TransactionsGrpc.TransactionsGrpcBase
{
    private readonly ITransactionService _transactionService;

    public TransactionGrpcService(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public override async Task<TransactionGrpcResponse> GetById(GetTransactionByIdGrpcRequest request, ServerCallContext context)
    {
        var response = await _transactionService.GetByIdAsync(Guid.Parse(request.Id), context.CancellationToken);
        
        var grpcResponse = response.ToGrpcResponse();
        
        return grpcResponse;
    }
}