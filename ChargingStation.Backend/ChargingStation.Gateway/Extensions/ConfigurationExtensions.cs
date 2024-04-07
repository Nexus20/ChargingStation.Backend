using Newtonsoft.Json.Linq;

namespace ChargingStation.Gateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddOcelotJsonFiles(this IConfigurationBuilder builder, string directory)
        {
            var mergedConfig = new JObject();

            var files = Directory.GetFiles(directory, "*.json");
            foreach (var file in files)
            {
                var json = JObject.Parse(File.ReadAllText(file));
                mergedConfig.Merge(json, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            var mergedJsonFilePath = $"{directory}\\OcelotSettings.json";
            File.WriteAllText(mergedJsonFilePath, mergedConfig.ToString());

            builder.AddJsonFile(mergedJsonFilePath, optional: false, reloadOnChange: true);

            return builder;
        }
    }
}
