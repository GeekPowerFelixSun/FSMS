﻿using FSMS.Entity.SystemManage;
using FSMS.Enum;
using FSMS.Model.Param.SystemManage;
using FSMS.Service.SystemManage;
using FSMS.Util;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.SystemManage
{
    public class AutoJobBLL
    {
        private AutoJobService autoJobService = new AutoJobService();

        #region 获取数据
        public async Task<TData<List<AutoJobEntity>>> GetList(AutoJobListParam param)
        {
            TData<List<AutoJobEntity>> obj = new TData<List<AutoJobEntity>>();
            obj.Result = await autoJobService.GetList(param);
            obj.TotalCount = obj.Result.Count;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<List<AutoJobEntity>>> GetPageList(AutoJobListParam param, Pagination pagination)
        {
            TData<List<AutoJobEntity>> obj = new TData<List<AutoJobEntity>>();
            obj.Result = await autoJobService.GetPageList(param, pagination);
            obj.TotalCount = pagination.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<AutoJobEntity>> GetEntity(long id)
        {
            TData<AutoJobEntity> obj = new TData<AutoJobEntity>();
            obj.Result = await autoJobService.GetEntity(id);
            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 提交数据
        public async Task<TData<string>> SaveForm(AutoJobEntity entity)
        {
            TData<string> obj = new TData<string>();
            if (autoJobService.ExistJob(entity))
            {
                obj.Message = "任务已经存在！";
                return obj;
            }
            await autoJobService.SaveForm(entity);
            obj.Result = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> DeleteForm(string ids)
        {
            TData<long> obj = new TData<long>();
            foreach (long id in TextHelper.SplitToArray<long>(ids, ','))
            {
                AutoJobEntity dbEntity = await autoJobService.GetEntity(id);
                if (dbEntity.JobStatus == StatusEnum.Yes.ParseToInt())
                {
                    obj.Message = "请先暂停 " + dbEntity.JobName + " 定时任务";
                    return obj;
                }
            }
            await autoJobService.DeleteForm(ids);

            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> ChangeJobStatus(AutoJobEntity entity)
        {
            TData obj = new TData();

            await autoJobService.SaveForm(entity);

            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 私有方法
        #endregion
    }
}
