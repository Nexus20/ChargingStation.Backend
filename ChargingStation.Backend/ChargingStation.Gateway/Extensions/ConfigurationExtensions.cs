namespace ChargingStation.Gateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddOcelotJsonFiles(this IConfigurationBuilder builder, string directory)
        {
            builder.AddJsonFile($"{directory}\\ChargePointSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\ChargingProfileSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\ConnectorSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\DepotSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\HeartbeatSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\OcppTagSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\ReservationSettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"{directory}\\TransactionSettings.json", optional: false, reloadOnChange: true);

            return builder;
        }
    }
}
