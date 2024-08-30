using System.Collections;
using System.Text.Json.Serialization;

namespace Ethik.Utility.Collections;

[Serializable]
public class PagedList<T> : IEnumerable<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 0;
    public bool HasNextPage => PageNumber < TotalPages - 1;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? NextPage { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? PreviousPage { get; set; }

    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
