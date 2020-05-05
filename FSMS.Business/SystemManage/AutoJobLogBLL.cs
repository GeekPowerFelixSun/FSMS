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
    public class AutoJobLogBLL
    {
        private AutoJobLogService autoJobLogService = new AutoJobLogService();

        #region 获取数据
        public async Task<TData<List<AutoJobLogEntity>>> GetList(AutoJobLogListParam param)
        {
            TData<List<AutoJobLogEntity>> obj = new TData<List<AutoJobLogEntity>>();
            obj.Result = await autoJobLogService.GetList(param);
            obj.TotalCount = obj.Result.Count;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<List<AutoJobLogEntity>>> GetPageList(AutoJobLogListParam param, Pagination pagination)
        {
            TData<List<AutoJobLogEntity>> obj = new TData<List<AutoJobLogEntity>>();
            obj.Result = await autoJobLogService.GetPageList(param, pagination);
            obj.TotalCount = pagination.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<AutoJobLogEntity>> GetEntity(long id)
        {
            TData<AutoJobLogEntity> obj = new TData<AutoJobLogEntity>();
            obj.Result = await autoJobLogService.GetEntity(id);
            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 提交数据
        public async Task<TData<string>> SaveForm(AutoJobLogEntity entity)
        {
            TData<string> obj = new TData<string>();
            await autoJobLogService.SaveForm(entity);
            obj.Result = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> DeleteForm(string ids)
        {
            TData<long> obj = new TData<long>();
            await autoJobLogService.DeleteForm(ids);
            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 私有方法
        #endregion
    }
}
