namespace retail_e_commerce.DTOs.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid? ParentId { get; set; }
    }
}
