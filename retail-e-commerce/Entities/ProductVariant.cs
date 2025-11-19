namespace retail_e_commerce.Entities
{
    public class ProductVariant
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string Sku { get; set; } = default!;
        public string? Color { get; set; }
        public string? Size { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDefault { get; set; }
    }
}
