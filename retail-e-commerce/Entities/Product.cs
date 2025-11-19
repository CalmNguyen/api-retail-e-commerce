namespace retail_e_commerce.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public decimal BasePrice { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Active, Inactive

        public string? ExtraAttributesJson { get; set; }

        public List<ProductVariant> Variants { get; set; } = new();
        public List<ProductImage> Images { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        // optimistic concurrency
        public byte[] RowVersion { get; set; } = default!;
    }
}
