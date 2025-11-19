namespace retail_e_commerce.DTOs.Product
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }

        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }

        public decimal BasePrice { get; set; }
        public string Status { get; set; } = default!;

        public Dictionary<string, string>? ExtraAttributes { get; set; }

        public List<ProductVariantDto> Variants { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();

        public string RowVersion { get; set; } = default!;
    }
}
