using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Services
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public DistributedCacheService(IDistributedCache cache, IConfiguration configuration)
        {
            _distributedCache = cache;
            _configuration = configuration;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            byte[] cacheData = await _distributedCache.GetAsync(key);
            if (cacheData != null)
            {
                return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cacheData));
            }

            return default(T);
        }

        public async Task SetAsync<T>(string key, T value, int timeDurationInHours = 0)
        {
            byte[] byteValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
            if (timeDurationInHours == 0)
            {
                timeDurationInHours = _configuration.GetValue<int>("CacheDurationInHours");
            }

            await _distributedCache.SetAsync(key, byteValue, new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(timeDurationInHours)));
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}