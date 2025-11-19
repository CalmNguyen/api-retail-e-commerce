using System.ComponentModel.DataAnnotations;

namespace retail_e_commerce.DTOs.Product
{
    public class ProductVariantDto
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Sku { get; set; } = default!;

        public string? Color { get; set; }
        public string? Size { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsDefault { get; set; }
    }
}
