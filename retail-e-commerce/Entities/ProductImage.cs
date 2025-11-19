namespace retail_e_commerce.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string Url { get; set; } = default!;
        public bool IsThumbnail { get; set; }
        public int SortOrder { get; set; }
    }
}
