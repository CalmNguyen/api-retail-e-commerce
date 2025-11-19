namespace retail_e_commerce.DTOs.Product
{
    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = default!;
    }
}
