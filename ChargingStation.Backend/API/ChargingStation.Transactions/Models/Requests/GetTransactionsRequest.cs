using ChargingStation.Common.Models.General.Requests;

namespace ChargingStation.Transactions.Models.Requests;

public class GetTransactionsRequest : BaseCollectionRequest
{
    public int? TransactionId { get; set; }
}