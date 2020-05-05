﻿using FSMS.Entity.SystemManage;
using FSMS.Model.Param.SystemManage;
using FSMS.Service.SystemManage;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.SystemManage
{
    public class DataDictDetailBLL
    {
        private DataDictDetailService dataDictDetailService = new DataDictDetailService();

        #region  获取数据
        public async Task<TData<List<DataDictDetailEntity>>> GetList(DataDictDetailListParam param)
        {
            TData<List<DataDictDetailEntity>> obj = new TData<List<DataDictDetailEntity>>();
            obj.Result = await dataDictDetailService.GetList(param);
            obj.TotalCount = obj.Result.Count;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<List<DataDictDetailEntity>>> GetPageList(DataDictDetailListParam param, Pagination pagination)
        {
            TData<List<DataDictDetailEntity>> obj = new TData<List<DataDictDetailEntity>>();
            obj.Result = await dataDictDetailService.GetPageList(param, pagination);
            obj.TotalCount = pagination.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<DataDictDetailEntity>> GetEntity(long id)
        {
            TData<DataDictDetailEntity> obj = new TData<DataDictDetailEntity>();
            obj.Result = await dataDictDetailService.GetEntity(id);
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<int>> GetMaxSort()
        {
            TData<int> obj = new TData<int>();
            obj.Result = await dataDictDetailService.GetMaxSort();
            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 提交数据
        public async Task<TData<string>> SaveForm(DataDictDetailEntity entity)
        {
            TData<string> obj = new TData<string>();
            if (entity.DictKey <= 0)
            {
                obj.Message = "字典键必须大于0";
                return obj;
            }
            if (dataDictDetailService.ExistDictKeyValue(entity))
            {
                obj.Message = "字典键或值已经存在！";
                return obj;
            }
            await dataDictDetailService.SaveForm(entity);
            obj.Result = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> DeleteForm(string ids)
        {
            TData<long> obj = new TData<long>();
            await dataDictDetailService.DeleteForm(ids);
            obj.Tag = 1;
            return obj;
        }
        #endregion
    }
}
