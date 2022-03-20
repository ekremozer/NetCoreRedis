using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreRedis.Web.Core.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        T GetOrCreate<T>(CacheKey cacheKey, Func<T> acquire);
        Task<T> GetOrCreateAsync<T>(CacheKey cacheKey, Func<T> acquire);
        byte[] Get(string key);
        Task<byte[]> GetAsync(string key);
        byte[] GetOrCreate(CacheKey cacheKey, Func<byte[]> acquire);
        Task<byte[]> GetOrCreateAsync(CacheKey cacheKey, Func<byte[]> acquire);
        string GetString(string key);
        Task<string> GetStringAsync(string key);
        string GetOrCreateString(CacheKey cacheKey, Func<string> acquire);
        Task<string> GetOrCreateStringAsync(CacheKey cacheKey, Func<string> acquire);
        void Set(CacheKey cacheKey, object model);
        Task SetAsync(CacheKey cacheKey, object model);
        void Set(CacheKey cacheKey, byte[] byteArray);
        Task SetAsync(CacheKey cacheKey, byte[] byteArray);
        void SetString(CacheKey cacheKey, string value);
        Task SetStringAsync(CacheKey cacheKey, string value);
        void Remove(string key);
        Task RemoveAsync(string key);
    }
}
