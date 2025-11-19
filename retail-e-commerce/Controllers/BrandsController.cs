using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using retail_e_commerce.Common;
using retail_e_commerce.Data;
using retail_e_commerce.DTOs.Brand;

namespace retail_e_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ICacheService _cache;

        private const string BrandsCacheKey = "brands:all";

        public BrandsController(AppDbContext db, ICacheService cache)
        {
            _db = db;
            _cache = cache;
        }

        // GET /api/brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll(bool isForceReload = false)
        {
            async Task<IEnumerable<BrandDto>> getListData()
            {
                var brands = await _db.Brands
                    .AsNoTracking()
                    .Select(b => new BrandDto
                    {
                        Id = b.Id,
                        Name = b.Name
                    })
                    .ToListAsync();
                return brands;
            }

            var brands = await _cache.GetOrCreateAsync(CacheConstantKey.GET_LIST_BRANDS, getListData, CacheConstant.MINUTE_DEFAULT_CACHE_30_MINUTE, forceReload: isForceReload);

            return Ok(brands);
        }
    }
}
