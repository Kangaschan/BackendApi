using LRA.Common.Models;

namespace LRA.Common.Utils;

public static class PaginationUtils
{
    public static PagedResult<T> Paginate<T>(
        this IEnumerable<T> source,
        int pageNumber,
        int pageSize)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
            
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be 1 or greater", nameof(pageNumber));
            
        if (pageSize < 1)
            throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var count = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count
        };
    }
}
