namespace retail_e_commerce.Entities
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;

        public List<Product> Products { get; set; } = new();
    }
}
