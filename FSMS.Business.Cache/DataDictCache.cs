﻿using FSMS.Cache.Factory;
using FSMS.Entity.SystemManage;
using FSMS.Service.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.Cache
{
    public class DataDictCache
    {
        private string cacheKey = typeof(DataDictCache).Name;

        private DataDictService dataDictService = new DataDictService();

        public async Task<List<DataDictEntity>> GetList()
        {
            var cacheList = CacheFactory.Cache().GetCache<List<DataDictEntity>>(cacheKey);
            if (cacheList == null)
            {
                var data = await dataDictService.GetList(null);
                var list = data.ToList();
                CacheFactory.Cache().AddCache(cacheKey, list);
                return list;
            }
            else
            {
                return cacheList;
            }
        }

        public void Remove()
        {
            CacheFactory.Cache().RemoveCache(cacheKey);
        }
    }
}
