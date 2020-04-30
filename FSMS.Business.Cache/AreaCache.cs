using FSMS.Cache.Factory;
using FSMS.Entity.SystemManage;
using FSMS.Service.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.Cache
{
    public class AreaCache
    {
        private string cacheKey = typeof(AreaCache).Name;

        private AreaService areaService = new AreaService();

        public async Task<List<AreaEntity>> GetList()
        {
            var cacheList = CacheFactory.Cache().GetCache<List<AreaEntity>>(cacheKey);
            if (cacheList == null)
            {
                var result = await areaService.GetList(null);
                var data = result.ToList();
                CacheFactory.Cache().AddCache(cacheKey, data);
                return data;
            }
            else
            {
                return cacheList;
            }
        }

        public void UpdateRow(long id)
        {
            //var cacheList = CacheFactory.Cache().GetCache<IEnumerable<AreaEntity>>(cacheKey);
            //if (cacheList != null)
            //{
            //    cacheList.Except(cacheList.Where(p => p.Id == id));
            //}
        }

        public void RemoveRow(long id)
        {

        }

    }
}
