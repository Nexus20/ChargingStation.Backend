using ChargingStation.Common.Models.Transactions.Responses;
using Transactions.Grpc;

namespace ChargingStation.Transactions.Grpc.Extensions;

public static class ResponseExtensions
{
    public static TransactionGrpcResponse ToGrpcResponse(this TransactionResponse response)
    {
        return new TransactionGrpcResponse
        {
            TransactionId = response.TransactionId
        };
    }
}