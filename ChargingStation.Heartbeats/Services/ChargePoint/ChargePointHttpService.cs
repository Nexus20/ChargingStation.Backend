namespace ChargingStation.Heartbeats.Services.Connectors
{
    public class ChargePointHttpService : IChargePointHttpService
    {
        private readonly HttpClient _httpClient;

        public ChargePointHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<bool> GetByIdAsync(string chargePointId, CancellationToken cancellationToken = default)
        {
            var requestUri = $"api/ChargePoint/{chargePointId}";
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            return response.IsSuccessStatusCode;
        }
    }
}
