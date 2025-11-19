namespace retail_e_commerce.DTOs
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int TotalItems { get; }
        public int Page { get; }
        public int PageSize { get; }

        public PagedResult(IReadOnlyList<T> items, int totalItems, int page, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
        }
    }
}
