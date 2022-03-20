using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreRedis.Web.Core.Caching
{
    public class CacheKey
    {
        #region Ctor
        public CacheKey(string key, int cacheTime)
        {
            Key = key;
            CacheTime = cacheTime;
        }
        public CacheKey(string key, int cacheTime, int cacheSlidingTime)
        {
            Key = key;
            CacheTime = cacheTime;
            CacheSlidingTime = cacheSlidingTime;
        }
        #endregion

        public string Key { get; protected set; }
        public int CacheTime { get; set; }
        public int? CacheSlidingTime { get; set; }
    }
}
