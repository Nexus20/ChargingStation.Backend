namespace ChargingStation.OcppTags.Helpers;

public class OcppTagHelper
{
    public static string CleanChargeTagId(string rawChargeTagId, ILogger logger)
    {
        var idTag = rawChargeTagId;

        // KEBA adds the serial to the idTag ("<idTag>_<serial>") => cut off suffix
        if (!string.IsNullOrWhiteSpace(rawChargeTagId))
        {
            var sep = rawChargeTagId.IndexOf('_');
            
            if (sep >= 0)
            {
                idTag = rawChargeTagId.Substring(0, sep);
                logger.LogTrace("CleanChargeTagId => Charge tag '{RawTag}' => '{ProcessedTag}'", rawChargeTagId, idTag);
            }
        }

        return idTag;
    }
}