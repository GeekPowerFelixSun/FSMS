using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using FSMS.Util;
using FSMS.Cache.Interface;

namespace FSMS.MemoryCache
{
    /// <summary>
    /// 01.对缓存的增删改查
    /// </summary>
    public class MemoryCacheImp:ICache
    {
        private IMemoryCache cache = GlobalContext.ServiceProvider.GetService<IMemoryCache>();

        public void AddCache<T>(string cacheKey, T value)
        {
            cache.Set(cacheKey, value, DateTimeOffset.Now.AddMinutes(10));
        }

        public void AddCache<T>(string cacheKey, T value, DateTime expireTime)
        {
            cache.Set(cacheKey, value, expireTime);
        }

        public void RemoveCache(string cacheKey)
        {
            cache.Remove(cacheKey);
        }

        public void RemoveAllCache()
        {

        }

        public T GetCache<T>(string cacheKey)
        {
            if (cache.Get(cacheKey) != null)
            {
                return (T)cache.Get(cacheKey);
            }
            else
            {
                return default(T);
            }
        }
    }
}
