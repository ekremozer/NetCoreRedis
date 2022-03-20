using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace NetCoreRedis.Web.Core.Caching
{
    public class RedisIDistributedCacheManager:ICacheManager
    {
        private readonly IDistributedCache _distributedCache;

        public RedisIDistributedCacheManager(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T Get<T>(string key)
        {
            var byteArray = _distributedCache.Get(key);
            var model = ByteArrayToModel<T>(byteArray);
            return model;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var byteArray = await _distributedCache.GetAsync(key);
            var model = ByteArrayToModel<T>(byteArray);
            return model;
        }

        public T GetOrCreate<T>(CacheKey cacheKey, Func<T> acquire)
        {
            if (cacheKey.CacheTime <= 0 && (cacheKey.CacheSlidingTime == null || cacheKey.CacheSlidingTime <= 0))
            {
                return acquire();
            }

            var cacheByteArray = _distributedCache.Get(cacheKey.Key);
            if (cacheByteArray == null)
            {
                var byteArray = acquire();
                if (byteArray != null)
                {
                    Set(cacheKey, byteArray);
                }
                return byteArray;
            }

            var model = ByteArrayToModel<T>(cacheByteArray);
            return model;
        }

        public async Task<T> GetOrCreateAsync<T>(CacheKey cacheKey, Func<T> acquire)
        {
            if (cacheKey.CacheTime <= 0 && (cacheKey.CacheSlidingTime == null || cacheKey.CacheSlidingTime <= 0))
            {
                return acquire();
            }

            var cacheByteArray = await _distributedCache.GetAsync(cacheKey.Key);
            if (cacheByteArray == null)
            {
                var byteArray = acquire();
                if (byteArray != null)
                {
                    await SetAsync(cacheKey, byteArray);
                }
                return byteArray;
            }

            var model = ByteArrayToModel<T>(cacheByteArray);
            return model;
        }

        public byte[] Get(string key)
        {
            var byteArray = _distributedCache.Get(key);
            return byteArray;
        }

        public async Task<byte[]> GetAsync(string key)
        {
            var byteArray = await _distributedCache.GetAsync(key);
            return byteArray;
        }

        public byte[] GetOrCreate(CacheKey cacheKey, Func<byte[]> acquire)
        {
            if (cacheKey.CacheTime <= 0 && (cacheKey.CacheSlidingTime == null || cacheKey.CacheSlidingTime <= 0))
            {
                return acquire();
            }

            var cacheByteArray = _distributedCache.Get(cacheKey.Key);
            if (cacheByteArray == null)
            {
                var byteArray = acquire();
                if (byteArray != null)
                {
                    Set(cacheKey, byteArray);
                }
                return byteArray;
            }
            return cacheByteArray;
        }

        public async Task<byte[]> GetOrCreateAsync(CacheKey cacheKey, Func<byte[]> acquire)
        {
            if (cacheKey.CacheTime <= 0 && (cacheKey.CacheSlidingTime == null || cacheKey.CacheSlidingTime <= 0))
            {
                return acquire();
            }

            var cacheByteArray = await _distributedCache.GetAsync(cacheKey.Key);
            if (cacheByteArray == null)
            {
                var byteArray = acquire();
                if (byteArray != null)
                {
                    await SetAsync(cacheKey, byteArray);
                }
                return byteArray;
            }
            return cacheByteArray;
        }

        public string GetString(string key)
        {
            var value = _distributedCache.GetString(key);
            return value;
        }

        public async Task<string> GetStringAsync(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);
            return value;
        }

        public string GetOrCreateString(CacheKey cacheKey, Func<string> acquire)
        {
            if (cacheKey.CacheTime <= 0 && (cacheKey.CacheSlidingTime == null || cacheKey.CacheSlidingTime <= 0))
            {
                return acquire();
            }

            var cacheValue = _distributedCache.GetString(cacheKey.Key);
            if (string.IsNullOrEmpty(cacheValue))
            {
                var value = acquire();
                if (string.IsNullOrEmpty(value))
                {
                    SetString(cacheKey, value);
                }
                return value;
            }
            return cacheValue;
        }

        public async Task<string> GetOrCreateStringAsync(CacheKey cacheKey, Func<string> acquire)
        {
            if (cacheKey.CacheTime <= 0 && (cacheKey.CacheSlidingTime == null || cacheKey.CacheSlidingTime <= 0))
            {
                return acquire();
            }

            var cacheValue = await _distributedCache.GetStringAsync(cacheKey.Key);
            if (string.IsNullOrEmpty(cacheValue))
            {
                var value = acquire();
                if (string.IsNullOrEmpty(value))
                {
                    await SetStringAsync(cacheKey, value);
                }
                return value;
            }
            return cacheValue;
        }

        public void Set(CacheKey cacheKey, object model)
        {
            var distributedCacheEntryOptions = PrepareDistributedCacheEntryOptions(cacheKey);
            var byteArray = ModelToByteArray(model);
            _distributedCache.Set(cacheKey.Key, byteArray, distributedCacheEntryOptions);
        }

        public Task SetAsync(CacheKey cacheKey, object model)
        {
            var memoryCacheEntryOptions = PrepareDistributedCacheEntryOptions(cacheKey);
            var byteArray = ModelToByteArray(model);
            _distributedCache.SetAsync(cacheKey.Key, byteArray, memoryCacheEntryOptions);
            return Task.CompletedTask;
        }

        public void Set(CacheKey cacheKey, byte[] byteArray)
        {
            var distributedCacheEntryOptions = PrepareDistributedCacheEntryOptions(cacheKey);
            _distributedCache.Set(cacheKey.Key, byteArray, distributedCacheEntryOptions);
        }

        public Task SetAsync(CacheKey cacheKey, byte[] byteArray)
        {
            var memoryCacheEntryOptions = PrepareDistributedCacheEntryOptions(cacheKey);
            _distributedCache.SetAsync(cacheKey.Key, byteArray, memoryCacheEntryOptions);
            return Task.CompletedTask;
        }

        public void SetString(CacheKey cacheKey, string value)
        {
            var memoryCacheEntryOptions = PrepareDistributedCacheEntryOptions(cacheKey);
            _distributedCache.SetString(cacheKey.Key, value, memoryCacheEntryOptions);
        }

        public Task SetStringAsync(CacheKey cacheKey, string value)
        {
            var memoryCacheEntryOptions = PrepareDistributedCacheEntryOptions(cacheKey);
            _distributedCache.SetStringAsync(cacheKey.Key, value, memoryCacheEntryOptions);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        public Task RemoveAsync(string key)
        {
            _distributedCache.Remove(key);
            return Task.CompletedTask;
        }

        private static DistributedCacheEntryOptions PrepareDistributedCacheEntryOptions(CacheKey cacheKey)
        {
            var distributedCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheKey.CacheTime)
            };

            if (cacheKey.CacheSlidingTime > 0)
            {
                distributedCacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes((int)cacheKey.CacheSlidingTime);
            }
            return distributedCacheEntryOptions;
        }

        private static byte[] ModelToByteArray<T>(T model)
        {
            if (model == null)
            {
                return null;
            }

            var jsonModel = JsonConvert.SerializeObject(model);
            var byteArray = Encoding.UTF8.GetBytes(jsonModel);

            return byteArray;
        }

        private static T ByteArrayToModel<T>(byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default;
            }

            var jsonModel = Encoding.UTF8.GetString(byteArray);
            var model = JsonConvert.DeserializeObject<T>(jsonModel);

            return model;
        }
    }
}
