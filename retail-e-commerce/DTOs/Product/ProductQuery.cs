namespace retail_e_commerce.DTOs.Product
{
    public class ProductQuery
    {
        private const int MaxPageSize = 100;

        public int Page { get; set; } = 1;

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Status { get; set; }
    }
}
