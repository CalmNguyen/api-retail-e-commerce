using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using retail_e_commerce.Common;
using retail_e_commerce.Data;
using retail_e_commerce.DTOs.Category;

namespace retail_e_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ICacheService _cache;

        private const string CategoriesCacheKey = "categories:all";

        public CategoriesController(AppDbContext db, ICacheService cache)
        {
            _db = db;
            _cache = cache;
        }

        // GET /api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(bool isForceReload=false)
        {
            async Task<IEnumerable<CategoryDto>> getAllData()
            {
                var categories = await _db.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId
                })
                .ToListAsync();
                return categories;
            }

            var categories = await _cache.GetOrCreateAsync(CacheConstantKey.GET_LIST_CATEGORIES, getAllData, CacheConstant.MINUTE_DEFAULT_CACHE_30_MINUTE, forceReload: isForceReload)

            return Ok(categories);
        }
    }
}
