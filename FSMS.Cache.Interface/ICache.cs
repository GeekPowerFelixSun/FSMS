using System;

namespace FSMS.Cache.Interface
{
    /// <summary>
    /// 01.缓存的通用接口，方便实现
    /// </summary>
    public interface ICache
    {
        void AddCache<T>(string cacheKey, T value);

        void AddCache<T>(string cacheKey, T value, DateTime expireTime);

        void RemoveCache(string cacheKey);

        void RemoveAllCache();

        T GetCache<T>(string cacheKey);

    }
}
