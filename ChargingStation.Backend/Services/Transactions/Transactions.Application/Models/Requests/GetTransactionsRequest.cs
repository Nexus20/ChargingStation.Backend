using ChargingStation.Common.Models.General.Requests;

namespace Transactions.Application.Models.Requests;

public class GetTransactionsRequest : BaseCollectionRequest
{
    public int? TransactionId { get; set; }
}