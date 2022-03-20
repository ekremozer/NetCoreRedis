using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace NetCoreRedis.Web.Core.Caching
{
    public class RedisStackExchangeCacheManager
    {
        private readonly ConnectionMultiplexer _redisConnector;
        public RedisStackExchangeCacheManager(IConfiguration configuration)
        {
            var redisServerUrl = configuration["RedisServerUrl"];
            _redisConnector = ConnectionMultiplexer.Connect(redisServerUrl);
        }

        public IDatabase GetDb(int dbIndex = -1)
        {
            return _redisConnector.GetDatabase(dbIndex);
        }
    }
}
