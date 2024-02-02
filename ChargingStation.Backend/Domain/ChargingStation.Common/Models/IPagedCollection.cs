namespace ChargingStation.Common.Models;

/// <summary>
/// Interface for paged collection used on the Data Access Layer.
/// </summary>
/// <typeparam name="TEntity">Type of the entity on which paged collection will be created.</typeparam>
public interface IPagedCollection<TEntity> : IEnumerable<TEntity>
{
    /// <summary>
    /// Entity collection.
    /// </summary>
    IEnumerable<TEntity> Collection { get; set; }

    /// <summary>
    /// Pages count.
    /// </summary>
    int PagesCount { get; set; }

    /// <summary>
    /// Current page.
    /// </summary>
    int CurrentPage { get; set; }

    /// <summary>
    /// Page size.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Elements total count.
    /// </summary>
    int ElementsTotalCount { get; set; }
}