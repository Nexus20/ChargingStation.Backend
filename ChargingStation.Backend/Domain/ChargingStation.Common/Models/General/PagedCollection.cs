using System.Collections;

namespace ChargingStation.Common.Models.General;

/// <inheritdoc cref="IPagedCollection{TEntity}"/>
public class PagedCollection<TEntity> : IPagedCollection<TEntity>
{
    public IEnumerable<TEntity> Collection { get; set; } = new List<TEntity>();
    public int PagesCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int ElementsTotalCount { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public PagedCollection()
    {
    }

    /// <summary>
    /// Parameterized constructor.
    /// </summary>
    /// <param name="collection">Collection of entities.</param>
    /// <param name="elementsTotalCount">Total count of entities.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="currentPage">Current page.</param>
    public PagedCollection(IEnumerable<TEntity> collection, int elementsTotalCount, int pageSize, int currentPage)
    {
        Collection = collection;
        ElementsTotalCount = elementsTotalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        PagesCount = pageSize == 0 ? 0 : (int)Math.Ceiling(elementsTotalCount / (double)pageSize);
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
        return Collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Collection.GetEnumerator();
    }
    
    public static IPagedCollection<TEntity> Empty => new PagedCollection<TEntity>();
}