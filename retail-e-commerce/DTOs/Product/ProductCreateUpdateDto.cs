using System.ComponentModel.DataAnnotations;

namespace retail_e_commerce.DTOs.Product
{
    public class ProductCreateUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        public Dictionary<string, string>? ExtraAttributes { get; set; }

        public List<ProductVariantDto> Variants { get; set; } = new();

        public List<string> ImageUrls { get; set; } = new();

        public string? RowVersion { get; set; }
    }
}
