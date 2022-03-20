using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreRedis.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NetCoreRedis.Web.Core.Caching;
using NetCoreRedis.Web.Core.FakeData;
using StackExchange.Redis;

namespace NetCoreRedis.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDatabase _redisDb;
        private readonly ICacheManager _cacheManager;
        public HomeController(RedisStackExchangeCacheManager redisStackExchangeCacheManager, ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _redisDb = redisStackExchangeCacheManager.GetDb();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RedisString()
        {
            //SET SiteAddress ekremozer.com
            _redisDb.StringSet("SiteAddress", "ekremozer.com");

            //ExpireTime Vermek için...
            _redisDb.StringSet("SiteAddress", "ekremozer.com", TimeSpan.FromMinutes(15));

            //GET SiteAddress
            var siteAddress = _redisDb.StringGet("SiteAddress");

            //GETRANGE SiteAddress 0 8
            var siteAddressRange = _redisDb.StringGetRange("SiteAddress", 0, 8);

            if (siteAddress.HasValue)
            {
                //Cache'de değer varsa...
            }

            if (siteAddress.IsNullOrEmpty)
            {
                //Değer null veya empty ise...
            }

            if (siteAddress.IsInteger)
            {
                //Değer integer tipindeyse...
            }

            if (siteAddress.IsNull)
            {
                //Değer null ise...
            }

            //Değeri stringe parse etmek için...
            var siteAddressValue = siteAddress.ToString();
            var siteAddressRangeValue = siteAddressRange.ToString();

            //SET UnitInStock 10
            _redisDb.StringSet("UnitInStock", 10);
            var unitInStock = _redisDb.StringGet("UnitInStock").ToString();

            //INCR UnitInStock
            _redisDb.StringIncrement("UnitInStock");
            unitInStock = _redisDb.StringGet("UnitInStock").ToString();

            //INCRBY UnitInStock 5
            _redisDb.StringIncrement("UnitInStock", 5);
            unitInStock = _redisDb.StringGet("UnitInStock").ToString();

            //DECR UnitInStock
            _redisDb.StringDecrement("UnitInStock");
            unitInStock = _redisDb.StringGet("UnitInStock").ToString();

            //DECRBY UnitInStock 5
            _redisDb.StringDecrement("UnitInStock", 5);
            unitInStock = _redisDb.StringGet("UnitInStock").ToString();

            //APPEND SiteAddress /hakkimda
            _redisDb.StringAppend("SiteAddress", "/hakkimda");
            siteAddressValue = _redisDb.StringGet("SiteAddress").ToString();

            return Content("Redis String");
        }

        public IActionResult RedisList()
        {
            //LPUSH Visitors Ekrem
            _redisDb.ListLeftPush("Visitors", "Ekrem");

            //Dizin olarak veri eklemek için...
            _redisDb.ListLeftPush("Visitors", new RedisValue[] { "Ekrem", "Hakan" });

            //ExpireTime Vermek için...
            _redisDb.KeyExpire("Visitors", TimeSpan.FromMinutes(15));

            //LRANGE Visitors 0 -1
            if (_redisDb.KeyExists("Visitors"))//Cache'de bu key'e ait veri varsa
            {
                var visitors = _redisDb.ListRange("Visitors");

                //RedisValue array'i list stringe parse etmek için...
                var stringVisitors = visitors.Select(item => item.ToString()).ToList();
            }

            //RPUSH Visitors Ozer
            _redisDb.ListRightPush("Visitors", "Ozer");
            var visitorsRightPush = _redisDb.ListRange("Visitors");

            //LINDEX Visitors 1
            var visitorByIndex = _redisDb.ListGetByIndex("Visitors", 1);

            //LPOP Visitors
            _redisDb.ListLeftPop("Visitors");
            var visitorsLeftPop = _redisDb.ListRange("Visitors");

            //RPOP Visitors
            _redisDb.ListRightPop("Visitors");
            var visitorsRightPop = _redisDb.ListRange("Visitors");

            //Listeden value'ye göre eleman silmek için...
            _redisDb.ListRemove("Visitors", "Ozer");
            var visitorsRemove = _redisDb.ListRange("Visitors");

            return Content("Redis List");
        }

        public IActionResult RedisSet()
        {
            //SADD MenuItems HomePage
            _redisDb.SetAdd("MenuItems", "HomePage");

            //Dizin olarak veri eklemek için...
            _redisDb.SetAdd("MenuItems", new RedisValue[] { "Blog", "Contact", "Search" });

            //ExpireTime Vermek için...
            _redisDb.KeyExpire("MenuItems", TimeSpan.FromMinutes(15));

            //SMEMBERS MenuItems
            if (_redisDb.KeyExists("MenuItems"))//Cache'de bu key'e ait veri varsa
            {
                var menuItems = _redisDb.SetMembers("MenuItems");

                //ListString türüne parse etmek için
                var stringMenuItems = menuItems.Select(item => item.ToString()).ToList();

                //HashSet türüne parse etmek için
                var hashSetMenuItems = new HashSet<string>();
                foreach (var item in menuItems)
                {
                    hashSetMenuItems.Add(item.ToString());
                }
            }

            //SREM MenuItems Search
            _redisDb.SetRemove("MenuItems", "Search");
            var menuItemsRemove = _redisDb.SetMembers("MenuItems");

            return Content("Redis Set");
        }

        public IActionResult RedisSortedSet()
        {
            //ZADD Categories 1 NetCore
            _redisDb.SortedSetAdd("Categories", "NetCore", 1);

            //ExpireTime Vermek için...
            _redisDb.KeyExpire("Categories", TimeSpan.FromMinutes(15));

            //Dizin olarak veri eklemek için
            var array = new[]
            {
                new SortedSetEntry("Sql", 4),
                new SortedSetEntry("NetMvc", 2),
                new SortedSetEntry("AspNet", 3)
            };
            _redisDb.SortedSetAdd("Categories", array);

            //ZRANGE Categories 0 -1
            //ZRANGE Categories 0 -1 WITHSCORES
            if (_redisDb.KeyExists("Categories"))//Cache'de bu key'e ait veri varsa
            {
                var categories = _redisDb.SortedSetScan("Categories").ToList();

                //Key ve value'yu ayrı ayrı okumak için
                foreach (var item in categories)
                {
                    var key = item.Key.ToString();
                    var value = item.Value;
                }

                //Aşağıda değerler score ile birlikte  Sql: 4 şeklide gelecektir.
                //ListString türüne parse etmek için
                var stringCategories = categories.Select(item => item.ToString()).ToList();

                //HashSet türüne parse etmek için
                var hashSetCategories = new HashSet<string>();
                foreach (var item in categories)
                {
                    hashSetCategories.Add(item.ToString());
                }

                //Küçükten büyüğe sıralama
                var orderByAscending = _redisDb.SortedSetRangeByRank("Categories", order: Order.Ascending);

                //Büyükten küçüğe sıralama
                var orderByDescending = _redisDb.SortedSetRangeByRank("Categories", order: Order.Descending);

                //Başlangıç ve bitiş indexine göre okuma
                var categoriesWithRange = _redisDb.SortedSetRangeByRank("Categories", 0, 2);
            }

            //ZREM Categories AspNet
            _redisDb.SortedSetRemove("Categories", "AspNet");
            var categoriesRemove = _redisDb.SortedSetScan("Categories").ToList();

            return Content("Redis Sorted Set");
        }

        public IActionResult RedisHash()
        {
            //HMSET ContactList Ekrem mail@ekremozer.com
            _redisDb.HashSet("ContactList", "Ekrem", "mail@ekremozer.com");

            //Dizin olarak veri eklemek için
            var array = new[]
            {
                new HashEntry("Hakan", "hakan@ekremozer.com"),
                new HashEntry("Info","info@ekremozer.com"),
            };
            _redisDb.HashSet("ContactList", array);

            //ExpireTime Vermek için...
            _redisDb.KeyExpire("ContactList", TimeSpan.FromMinutes(15));

            if (_redisDb.KeyExists("ContactList"))//Cache'de bu key'e ait veri varsa
            {
                //HGET ContactList Ekrem
                var contactListItem = _redisDb.HashGet("ContactList", "Ekrem");



                if (contactListItem.HasValue)
                {
                    //Cache'de değer varsa...
                }

                if (contactListItem.IsNullOrEmpty)
                {
                    //Değer null veya empty ise...
                }

                if (contactListItem.IsInteger)
                {
                    //Değer integer tipindeyse...
                }

                if (contactListItem.IsNull)
                {
                    //Değer null ise...
                }

                //Değeri stringe parse etmek için...
                var contactListItemString = contactListItem.ToString();

                //HGETALL ContactList
                var contactList = _redisDb.HashGetAll("ContactList");

                //Dictionary<string,string> türüne parse etmek için...
                var contactListDictionary = contactList.ToDictionary<HashEntry, string, string>(item => item.Key, item => item.Value);

                //HDEL ContactList Hakan
                _redisDb.HashDelete("ContactList", "Hakan ");

                var contactListDel = _redisDb.HashGetAll("ContactList");
            }

            return Content("Redis Hash");
        }

        public async Task<IActionResult> DistributedCache()
        {
            var personals = FakeDataGenerator.GetPersonals();
            var fileByteArray = FakeDataGenerator.GetFileByteArray();

            //CacheKey oluşturma
            var cacheKey = new CacheKey("Personals", 15);

            //SlidingTime ile CacheKey oluşturma
            var cacheKeySlidingTime = new CacheKey("Image", 15, 3);

            //Nesneleri cachelemek için...
            _cacheManager.Set(cacheKey, personals);
            await _cacheManager.SetAsync(cacheKey, personals);

            //Fiziksel dosyaları byte[] a dönüştürüp cachlemek için
            _cacheManager.Set(cacheKeySlidingTime, fileByteArray);
            await _cacheManager.SetAsync(cacheKeySlidingTime, fileByteArray);

            //Cacheden nesne getirmek için
            var personalsCache = _cacheManager.Get<List<Personal>>("Personals");
            var personalsCacheAsync = await _cacheManager.GetAsync<List<Personal>>("Personals");

            var imageByteArray = _cacheManager.Get("Image");
            var imageByteArrayAsync = _cacheManager.GetAsync("Image");

            //Image dosyasını actionda dönemk için...
            //return File(imageByteArray, "image/png");

            //Pdf dosyalarını actionda dönmek için...
            //return File(imageByteArray, "application/pdf"); 


            //Nesneleri varsa cacheden getirmek yoksa, fonksiyon ile oluşturup cachlemek için...
            var personalsGetOrCreate = _cacheManager.GetOrCreate(cacheKey, () => FakeDataGenerator.GetPersonals());
            var personalsGetOrCreateAsync = await _cacheManager.GetOrCreateAsync(cacheKey, FakeDataGenerator.GetPersonals);//Paremetre almayan metodları bu şekildede kullanabilirsiniz...

            //byte[] nesneleri varsa cacheden getirmek yoksa, fonksiyon ile oluşturup cachlemek için...
            var imageByteArrayGetOrCreate = _cacheManager.GetOrCreate(cacheKeySlidingTime, () => FakeDataGenerator.GetFileByteArray());
            var imageByteArrayGetOrCreateAsync = await _cacheManager.GetOrCreateAsync(cacheKeySlidingTime, () => FakeDataGenerator.GetFileByteArray());

            var cacheKeyString = new CacheKey("PersonalFullName", 15);
            var personalFullName = FakeDataGenerator.GetPersonalFullName(1);
            //String cachelemek için...
            _cacheManager.SetString(cacheKeyString, personalFullName);
            await _cacheManager.SetStringAsync(cacheKeyString, personalFullName);
            //String cacheden okumak için
            var personalFullNameCache = _cacheManager.GetString("PersonalFullName");
            var personalFullNameCacheAsync = await _cacheManager.GetStringAsync("PersonalFullName");

            //String nesneleri varsa cacheden getirmek yoksa, fonksiyon ile oluşturup cachlemek için...
            var personalFullNameGetOrCreate = _cacheManager.GetOrCreateString(cacheKeyString, () => FakeDataGenerator.GetPersonalFullName(1));
            var personalFullNameGetOrCreateCacheAsync = await _cacheManager.GetOrCreateStringAsync(cacheKeyString, () => FakeDataGenerator.GetPersonalFullName(1));

            //Cacheden veri silmek için
            _cacheManager.Remove("PersonalFullName");
            await _cacheManager.RemoveAsync("PersonalFullName");

            return Content("Redis IDistributed Cache");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
