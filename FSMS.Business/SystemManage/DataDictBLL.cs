﻿using FSMS.Entity.SystemManage;
using FSMS.Model.Param.SystemManage;
using FSMS.Model.Result.SystemManage;
using FSMS.Service.SystemManage;
using FSMS.Util;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.SystemManage
{
    public class DataDictBLL
    {
        private DataDictService dataDictService = new DataDictService();
        private DataDictDetailService dataDictDetailService = new DataDictDetailService();

        #region 获取数据
        public async Task<TData<List<DataDictEntity>>> GetList(DataDictListParam param)
        {
            TData<List<DataDictEntity>> obj = new TData<List<DataDictEntity>>();
            obj.Result = await dataDictService.GetList(param);
            obj.TotalCount = obj.Result.Count;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<List<DataDictEntity>>> GetPageList(DataDictListParam param, Pagination pagination)
        {
            TData<List<DataDictEntity>> obj = new TData<List<DataDictEntity>>();
            obj.Result = await dataDictService.GetPageList(param, pagination);
            obj.TotalCount = pagination.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<DataDictEntity>> GetEntity(long id)
        {
            TData<DataDictEntity> obj = new TData<DataDictEntity>();
            obj.Result = await dataDictService.GetEntity(id);
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<int>> GetMaxSort()
        {
            TData<int> obj = new TData<int>();
            obj.Result = await dataDictService.GetMaxSort();
            obj.Tag = 1;
            return obj;
        }

        /// <summary>
        /// 获取所有的数据字典
        /// </summary>
        /// <returns></returns>
        public async Task<TData<List<DataDictInfo>>> GetDataDictList()
        {
            TData<List<DataDictInfo>> obj = new TData<List<DataDictInfo>>();

            List<DataDictEntity> dataDictList = await dataDictService.GetList(null);
            List<DataDictDetailEntity> dataDictDetailList = await dataDictDetailService.GetList(null);

            List<DataDictInfo> dataDictInfoList = new List<DataDictInfo>();
            foreach (DataDictEntity dataDict in dataDictList)
            {
                List<DataDictDetailInfo> detailList = dataDictDetailList.Where(p => p.DictType == dataDict.DictType).OrderBy(p => p.DictSort).Select(p => new DataDictDetailInfo
                {
                    DictKey = p.DictKey,
                    DictValue = p.DictValue,
                    ListClass = p.ListClass,
                    IsDefault = p.IsDefault,
                    DictStatus = p.DictStatus,
                    Remark = p.Remark
                }).ToList();
                dataDictInfoList.Add(new DataDictInfo
                {
                    DictType = dataDict.DictType,
                    Detail = detailList
                });
            }
            obj.Result = dataDictInfoList;
            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 提交数据 
        public async Task<TData<string>> SaveForm(DataDictEntity entity)
        {
            TData<string> obj = new TData<string>();
            if (dataDictService.ExistDictType(entity))
            {
                obj.Message = "字典类型已经存在！";
                return obj;
            }

            await dataDictService.SaveForm(entity);

            obj.Result = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> DeleteForm(string ids)
        {
            TData obj = new TData();
            foreach (long id in TextHelper.SplitToArray<long>(ids, ','))
            {
                DataDictEntity dbEntity = await dataDictService.GetEntity(id);
                if (dataDictService.ExistDictDetail(dbEntity.DictType))
                {
                    obj.Message = "请先删除字典值！";
                    return obj;
                }
            }
            await dataDictService.DeleteForm(ids);
            obj.Tag = 1;
            return obj;
        }
        #endregion
    }
}
