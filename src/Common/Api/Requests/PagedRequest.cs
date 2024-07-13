namespace Chirper.Common.Api.Requests;

public interface IPagedRequest
{
    public const int MaxPageSize = 100;
    int? Page { get; }
    int? PageSize { get; }
}

public class PagedRequestValidator<T> : AbstractValidator<T> where T : IPagedRequest
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(IPagedRequest.MaxPageSize);
    }
}

public record PagedList<T>(List<T> Items, int Page, int PageSize, int TotalItems)
{
    public bool HasNextPage => Page * PageSize < TotalItems;
    public bool HasPreviousPage => Page > 1;
}

public static class PaginationDatabaseExtensions
{
    public static async Task<PagedList<TResponse>> ToPagedListAsync<TRequest, TResponse>(this IQueryable<TResponse> query, TRequest request, CancellationToken cancellationToken = default) where TRequest : IPagedRequest
    {
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, IPagedRequest.MaxPageSize);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<TResponse>(items, page, pageSize, totalItems);
    }
}