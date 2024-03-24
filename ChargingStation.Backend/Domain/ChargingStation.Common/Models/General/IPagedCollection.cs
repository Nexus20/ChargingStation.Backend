using Newtonsoft.Json;

namespace ChargingStation.Common.Models.General;

[JsonObject]
public interface IPagedCollection<T> : IEnumerable<T>
{
    [JsonProperty]
    IEnumerable<T> Collection { get; set; }

    [JsonProperty]
    int PagesCount { get; set; }

    [JsonProperty]
    int CurrentPage { get; set; }

    [JsonProperty]
    int PageSize { get; set; }

    [JsonProperty]
    int ElementsTotalCount { get; set; }
}