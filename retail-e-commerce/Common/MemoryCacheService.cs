using Microsoft.Extensions.Caching.Memory;

namespace retail_e_commerce.Common
{
    public interface ICacheService
    {
        /// <summary>
        /// Lưu giá trị vào cache với key, thời gian sống tính bằng phút.
        /// </summary>
        Task SetAsync<T>(string key, T value, int minutes);

        /// <summary>
        /// Lấy giá trị từ cache với kiểu dữ liệu T.
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Xoá một giá trị khỏi cache.
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Kiểm tra xem key có tồn tại trong cache không.
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Lấy giá trị từ cache hoặc đọc từ nguồn nếu không có trong cache.
        /// </summary>
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> dataRetrievalFunction, int minutes, bool forceReload = false);
    }
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task SetAsync<T>(string key, T value, int minutes)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            };
            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return Task.FromResult((T?)value);
            }
            return Task.FromResult<T?>(default);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> dataRetrievalFunction, int minutes, bool forceReload = false)
        {
            // Kiểm tra xem cache có key hay không
            if (!forceReload && _cache.TryGetValue(key, out var cachedValue))
            {
                return (T?)cachedValue;
            }

            // Nếu không có hoặc yêu cầu reload, gọi hàm async để lấy dữ liệu
            var value = await dataRetrievalFunction();
            try
            {
                _cache.Remove(key);
            }
            catch { }
            // Lưu vào cache
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            };

            _cache.Set(key, value, options);
            return value;
        }
    }
}
