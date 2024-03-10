using ChargingStation.Common.Models.Requests;

namespace ChargingStation.Transactions.Models.Requests;

public class GetTransactionsRequest : BaseCollectionRequest
{
    public int? TransactionId { get; set; }
}