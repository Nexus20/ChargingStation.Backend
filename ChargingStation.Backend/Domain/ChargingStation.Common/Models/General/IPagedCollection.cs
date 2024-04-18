using System.Diagnostics.CodeAnalysis;
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

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)]this IEnumerable<T>? enumerable)
    {
        return enumerable is null || !enumerable.Any();
    }
}

public static class PagedCollectionExtensions
{
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)]this IPagedCollection<T>? pagedCollection)
    {
        return pagedCollection?.Collection is null || !pagedCollection.Collection.Any();
    }
}