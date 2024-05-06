using ChargingStation.Common.Models.Transactions.Responses;
using ChargingStation.InternalCommunication.Extensions;
using Transactions.Grpc;

namespace ChargingStation.InternalCommunication.GrpcClients;

public class TransactionGrpcClientService
{
    private readonly TransactionsGrpc.TransactionsGrpcClient _transactionsGrpcClient;

    public TransactionGrpcClientService(TransactionsGrpc.TransactionsGrpcClient transactionsGrpcClient)
    {
        _transactionsGrpcClient = transactionsGrpcClient;
    }
    
    public async Task<TransactionResponse> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var grpcRequest = new GetTransactionByIdGrpcRequest
        {
            Id = transactionId.ToString()
        };

        var grpcResponse = await _transactionsGrpcClient.GetByIdAsync(grpcRequest, cancellationToken: cancellationToken);
        
        var response = grpcResponse.ToResponse();
        return response;
    }
}