using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using retail_e_commerce.Data;
using retail_e_commerce.DTOs.Product;
using retail_e_commerce.DTOs;
using retail_e_commerce.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace retail_e_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ProductsController(AppDbContext db, IMapper mapper, IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _cache = cache;
        }

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetProducts([FromQuery] ProductQuery query)
        {
            var cacheKey = $"products:{query.Page}:{query.PageSize}:{query.Keyword}:{query.CategoryId}:{query.BrandId}:{query.MinPrice}:{query.MaxPrice}:{query.Status}";

            if (_cache.TryGetValue(cacheKey, out PagedResult<ProductListItemDto>? cached))
            {
                return Ok(cached);
            }

            var q = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                q = q.Where(p => p.Name.Contains(query.Keyword));
            }

            if (query.CategoryId is not null && query.CategoryId != Guid.Empty)
            {
                q = q.Where(p => p.CategoryId == query.CategoryId);
            }

            if (query.BrandId is not null && query.BrandId != Guid.Empty)
            {
                q = q.Where(p => p.BrandId == query.BrandId);
            }

            if (query.MinPrice is not null)
            {
                q = q.Where(p => p.BasePrice >= query.MinPrice);
            }

            if (query.MaxPrice is not null)
            {
                q = q.Where(p => p.BasePrice <= query.MaxPrice);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                q = q.Where(p => p.Status == query.Status);
            }

            var totalItems = await q.CountAsync();

            var items = await q
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<ProductListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var result = new PagedResult<ProductListItemDto>(items, totalItems, query.Page, query.PageSize);

            return Ok(result);
        }

        // GET /api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductById(Guid id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var dto = _mapper.Map<ProductDetailDto>(product);
            return Ok(dto);
        }

        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<ProductDetailDto>> CreateProduct([FromBody] ProductCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                var product = _mapper.Map<Product>(dto);

                product.Id = Guid.NewGuid();
                product.Slug = GenerateSlug(product.Name);
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                product.IsDeleted = false;
                product.CreatedBy = 0;
                product.UpdatedBy = 0;

                // Variants
                foreach (var variantDto in dto.Variants)
                {
                    var variant = _mapper.Map<ProductVariant>(variantDto);
                    variant.Id = Guid.NewGuid();
                    variant.ProductId = product.Id;
                    product.Variants.Add(variant);
                }

                // Images
                foreach (var url in dto.ImageUrls)
                {
                    product.Images.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Url = url,
                        IsThumbnail = false,
                        SortOrder = 0
                    });
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                var resultDto = _mapper.Map<ProductDetailDto>(product);

                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, resultDto);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        // PUT /api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductDetailDto>> UpdateProduct(Guid id, [FromBody] ProductCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var product = await _db.Products
                //.Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.RowVersion))
            {
                var clientRowVersion = Convert.FromBase64String(dto.RowVersion);
                if (!product.RowVersion.SequenceEqual(clientRowVersion))
                {
                    return Conflict(new
                    {
                        Message = "The product was modified by another request. Please reload and try again."
                    });
                }
            }

            product.Name = dto.Name;
            product.Slug = GenerateSlug(dto.Name);
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;
            product.BasePrice = dto.BasePrice;
            product.Status = dto.Status;
            product.UpdatedAt = DateTime.UtcNow;

            // ExtraAttributes
            if (dto.ExtraAttributes != null)
            {
                product.ExtraAttributesJson = System.Text.Json.JsonSerializer.Serialize(dto.ExtraAttributes);
            }
            else
            {
                product.ExtraAttributesJson = null;
            }

            var oldVariants = product.Variants.ToList();

            var dictBySku = oldVariants.ToDictionary(v => v.Sku, v => v, StringComparer.OrdinalIgnoreCase);

            foreach (var variantDto in dto.Variants)
            {
                if (dictBySku.TryGetValue(variantDto.Sku, out var existing))
                {
                    existing.Color = variantDto.Color;
                    existing.Size = variantDto.Size;
                    existing.Price = variantDto.Price;
                    existing.StockQuantity = variantDto.StockQuantity;
                    existing.IsDefault = variantDto.IsDefault;
                }
                else
                {
                    var newVariant = _mapper.Map<ProductVariant>(variantDto);
                    newVariant.Id = Guid.NewGuid();
                    newVariant.ProductId = product.Id;
                    product.Variants.Add(newVariant);
                }
            }

            if (product.Variants.Any())
            {
                var anyDefault = product.Variants.Any(v => v.IsDefault);
                if (!anyDefault)
                {
                    product.Variants.First().IsDefault = true;
                }
            }

            // Update Images (simple approach)
            _db.ProductImages.RemoveRange(product.Images);
            product.Images.Clear();

            foreach (var url in dto.ImageUrls)
            {
                product.Images.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Url = url,
                    IsThumbnail = false,
                    SortOrder = 0
                });
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entities = ex.Entries
                    .Select(e => e.Entity.GetType().Name)
                    .Distinct()
                    .ToList();

                return Conflict(new
                {
                    Message = "Concurrency entities: " + string.Join(", ", entities),
                    Entities = entities
                });
            }


            var resultDto = _mapper.Map<ProductDetailDto>(product);
            return Ok(resultDto);
        }

        // DELETE /api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static string GenerateSlug(string name)
        {
            var slug = name.Trim().ToLowerInvariant();
            slug = slug.Replace(" ", "-");
            return slug;
        }
    }
}
