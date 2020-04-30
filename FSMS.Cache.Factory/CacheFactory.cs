using FSMS.Cache.Interface;
using FSMS.MemoryCache;

namespace FSMS.Cache.Factory
{
    /// <summary>
    /// 01.缓存的数据容器
    /// </summary>
    public class CacheFactory
    {
        public static ICache Cache()
        {
            return new MemoryCacheImp();
        }
    }
}
