using System.Collections;
using System.Text.Json.Serialization;

namespace Ethik.Utility.Data.Collections;

/// <summary>
/// Represents a paginated list of items with metadata about the current page, total count, and navigation links.
/// </summary>
/// <typeparam name="T">The type of items contained in the paginated list.</typeparam>
[Serializable]
public sealed class PagedList<T> : IEnumerable<T>
{
    /// <summary>
    /// Gets or sets the collection of items on the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number (zero-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 0;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages - 1;

    /// <summary>
    /// Gets or sets the URI for the next page, if applicable.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? NextPage { get; set; }

    /// <summary>
    /// Gets or sets the URI for the previous page, if applicable.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? PreviousPage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class with the specified items and pagination metadata.
    /// </summary>
    /// <param name="items">The collection of items on the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageNumber">The current page number (zero-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of items on the current page.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection of items.</returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection of items on the current page.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection of items.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}