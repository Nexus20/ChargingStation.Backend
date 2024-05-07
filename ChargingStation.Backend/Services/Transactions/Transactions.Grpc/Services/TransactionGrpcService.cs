using Transactions.Grpc.Extensions;
using Grpc.Core;
using Transactions.Application.Services.Transactions;
using Transactions.Grpc.Protos;

namespace Transactions.Grpc.Services;

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