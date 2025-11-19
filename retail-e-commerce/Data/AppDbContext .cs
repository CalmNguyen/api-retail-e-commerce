using Microsoft.EntityFrameworkCore;
using retail_e_commerce.Entities;

namespace retail_e_commerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Slug)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                //entity.Property(p => p.RowVersion)
                //    .IsRowVersion().IsConcurrencyToken(false); ;

                entity.HasQueryFilter(p => !p.IsDeleted);
            });

            // Category self reference
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId);

            // ProductVariant
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.Property(v => v.Sku)
                    .IsRequired()
                    .HasMaxLength(100);

                //entity.HasIndex(v => v.Sku)
                //    .IsUnique();
            });

            // ProductImage
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(i => i.Url)
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }
    }

}
