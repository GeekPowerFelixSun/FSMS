﻿using FSMS.Data;
using FSMS.Data.Repository;
using FSMS.Entity.SystemManage;
using FSMS.Model.Param.SystemManage;
using FSMS.Util;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Service.SystemManage
{
    public class LogLoginService : RepositoryFactory
    {
        #region 获取数据
        public async Task<List<LogLoginEntity>> GetList(LogLoginListParam param)
        {
            var strSql = new StringBuilder();
            List<DbParameter> filter = ListFilter(param, strSql);
            var list = await this.BaseRepository().FindList<LogLoginEntity>(strSql.ToString(), filter.ToArray());
            return list.ToList();
        }

        public async Task<List<LogLoginEntity>> GetPageList(LogLoginListParam param, Pagination pagination)
        {
            var strSql = new StringBuilder();
            List<DbParameter> filter = ListFilter(param, strSql);
            var list = await this.BaseRepository().FindList<LogLoginEntity>(strSql.ToString(), filter.ToArray(), pagination);
            return list.ToList();
        }

        public async Task<LogLoginEntity> GetEntity(long id)
        {
            return await this.BaseRepository().FindEntity<LogLoginEntity>(id);
        }
        #endregion

        #region 提交数据
        public async Task SaveForm(LogLoginEntity entity)
        {
            if (entity.Id.IsNullOrZero())
            {
                await entity.Create();
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                await this.BaseRepository().Update(entity);
            }
        }

        public async Task DeleteForm(string ids)
        {
            long[] idArr = TextHelper.SplitToArray<long>(ids, ',');
            await this.BaseRepository().Delete<LogLoginEntity>(idArr);
        }

        public async Task RemoveAllForm()
        {
            await this.BaseRepository().ExecuteBySql("truncate table sys_log_login");
        }
        #endregion

        #region 私有方法
        private List<DbParameter> ListFilter(LogLoginListParam param, StringBuilder strSql)
        {
            strSql.Append(@"SELECT  a.id as Id,
                                    a.base_create_time as BaseCreateTime,
                                    a.base_creator_id as BaseCreatorId,
                                    a.log_status as LogStatus,
                                    a.ip_address as IpAddress,
                                    a.ip_location as IpLocation,
                                    a.browser as Browser,
                                    a.os as OS,
                                    a.remark as Remark,
                                    b.user_name as UserName
                            FROM    sys_log_login a
                                    LEFT JOIN sys_user b ON a.base_creator_id = b.id
                            WHERE   1 = 1");
            var parameter = new List<DbParameter>();
            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.UserName))
                {
                    strSql.Append(" AND b.user_name like @UserName");
                    parameter.Add(DbParameterExtension.CreateDbParameter("@UserName", '%' + param.UserName + '%'));
                }
                if (param.LogStatus > -1)
                {
                    strSql.Append(" AND a.log_status = @LogStatus");
                    parameter.Add(DbParameterExtension.CreateDbParameter("@LogStatus", param.LogStatus));
                }
                if (!string.IsNullOrEmpty(param.IpAddress))
                {
                    strSql.Append(" AND a.ip_address like @IpAddress");
                    parameter.Add(DbParameterExtension.CreateDbParameter("@IpAddress", '%' + param.IpAddress + '%'));
                }
                if (!string.IsNullOrEmpty(param.StartTime.ParseToString()))
                {
                    strSql.Append(" AND a.base_create_time >= @StartTime");
                    parameter.Add(DbParameterExtension.CreateDbParameter("@StartTime", param.StartTime));
                }
                if (!string.IsNullOrEmpty(param.EndTime.ParseToString()))
                {
                    param.EndTime = (param.EndTime.Value.ToString("yyyy-MM-dd") + " 23:59:59").ParseToDateTime();
                    strSql.Append(" AND a.base_create_time <= @EndTime");
                    parameter.Add(DbParameterExtension.CreateDbParameter("@EndTime", param.EndTime));
                }
            }
            return parameter;
        }
        #endregion
    }
}
