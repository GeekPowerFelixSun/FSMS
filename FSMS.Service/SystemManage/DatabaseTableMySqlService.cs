﻿using FSMS.Data;
using FSMS.Data.EF;
using FSMS.Data.Repository;
using FSMS.Entity.OrganizationManage;
using FSMS.Entity.SystemManage;
using FSMS.Model.Result.SystemManage;
using FSMS.Util;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Service.SystemManage
{
    public class DatabaseTableMySqlService : RepositoryFactory, IDatabaseTableService
    {
        #region 获取数据
        public async Task<List<TableInfo>> GetTableList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT table_name TableName FROM information_schema.tables WHERE table_schema='" + GetDatabase() + "' AND table_type='base table'");
            IEnumerable<TableInfo> list = await this.BaseRepository().FindList<TableInfo>(strSql.ToString());
            if (!string.IsNullOrEmpty(tableName))
            {
                list = list.Where(p => p.TableName.Contains(tableName));
            }
            SetTableDetail(list);
            return list.ToList();
        }

        public async Task<List<TableInfo>> GetTablePageList(string tableName, Pagination pagination)
        {
            StringBuilder strSql = new StringBuilder();
            var parameter = new List<DbParameter>();
            strSql.Append(@"SELECT table_name TableName FROM information_schema.tables where table_schema='" + GetDatabase() + "' and (table_type='base table' or table_type='BASE TABLE')");

            if (!string.IsNullOrEmpty(tableName))
            {
                strSql.Append(" AND table_name like @TableName ");
                parameter.Add(DbParameterExtension.CreateDbParameter("@TableName", '%' + tableName + '%'));
            }

            IEnumerable<TableInfo> list = await this.BaseRepository().FindList<TableInfo>(strSql.ToString(), parameter.ToArray(), pagination);
            SetTableDetail(list);
            return list.ToList();
        }

        public async Task<List<TableFieldInfo>> GetTableFieldList(string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT COLUMN_NAME TableColumn, 
		                           DATA_TYPE Datatype,
		                           (CASE COLUMN_KEY WHEN 'PRI' THEN COLUMN_NAME ELSE '' END) TableIdentity,
		                           REPLACE(REPLACE(SUBSTRING(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)),'(',''),')','') FieldLength,
	                               (CASE IS_NULLABLE WHEN 'NO' THEN 'N' ELSE 'Y' END) IsNullable,
                                   IFNULL(COLUMN_DEFAULT,'') FieldDefault,
                                   COLUMN_COMMENT Remark
                             FROM information_schema.columns WHERE table_schema='" + GetDatabase() + "' AND table_name=@TableName");
            var parameter = new List<DbParameter>();
            parameter.Add(DbParameterExtension.CreateDbParameter("@TableName", tableName));
            var list = await this.BaseRepository().FindList<TableFieldInfo>(strSql.ToString(), parameter.ToArray());
            return list.ToList();
        }
        #endregion

        #region 公有方法
        public async Task<bool> DatabaseBackup(string database, string backupPath)
        {
            string backupFile = string.Format("{0}\\{1}_{2}.bak", backupPath, database, DateTime.Now.ToString("yyyyMMddHHmmss"));
            string strSql = string.Format(" backup database [{0}] to disk = '{1}'", database, backupFile);
            var result = await this.BaseRepository().ExecuteBySql(strSql);
            return result > 0 ? true : false;
        }

        /// <summary>
        /// 仅用在框架里面，同步不同数据库之间的数据，以 MySql 为主库，同步 MySql 的数据到SqlServer和Oracle，保证各个数据库的数据是一样的
        /// </summary>
        /// <returns></returns>
        public async Task SyncDatabase()
        {
            #region 同步SqlServer数据库
            await SyncSqlServerTable<AreaEntity>();
            await SyncSqlServerTable<AutoJobEntity>();
            await SyncSqlServerTable<AutoJobLogEntity>();
            await SyncSqlServerTable<DataDictEntity>();
            await SyncSqlServerTable<DataDictDetailEntity>();
            await SyncSqlServerTable<DepartmentEntity>();
            await SyncSqlServerTable<LogLoginEntity>();
            await SyncSqlServerTable<MenuEntity>();
            await SyncSqlServerTable<MenuAuthorizeEntity>();
            await SyncSqlServerTable<NewsEntity>();
            await SyncSqlServerTable<PositionEntity>();
            await SyncSqlServerTable<RoleEntity>();
            await SyncSqlServerTable<UserEntity>();
            await SyncSqlServerTable<UserBelongEntity>();
            #endregion
        }
        private async Task SyncSqlServerTable<T>() where T : class, new()
        {
            string sqlServerConnectionString = "Server=localhost;Database=yisha_admin;User Id=sa;Password=Atser123;";
            IEnumerable<T> list = await this.BaseRepository().FindList<T>();

            await new SqlServerDatabase(sqlServerConnectionString).Delete<T>(p => true);
            await new SqlServerDatabase(sqlServerConnectionString).Insert<T>(list);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取所有表的主键、主键名称、记录数
        /// </summary>
        /// <returns></returns>
        private async Task<List<TableInfo>> GetTableDetailList()
        {
            string strSql = @"SELECT t1.TABLE_NAME TableName,t1.TABLE_COMMENT Remark,t1.TABLE_ROWS TableCount,t2.CONSTRAINT_NAME TableKeyName,t2.column_name TableKey
                                     FROM information_schema.TABLES as t1 
	                                 LEFT JOIN INFORMATION_SCHEMA.`KEY_COLUMN_USAGE` as t2 on t1.TABLE_NAME = t2.TABLE_NAME
                                     WHERE t1.TABLE_SCHEMA='" + GetDatabase() + "' AND t2.TABLE_SCHEMA='" + GetDatabase() + "'";

            IEnumerable<TableInfo> list = await this.BaseRepository().FindList<TableInfo>(strSql.ToString());
            return list.ToList();
        }

        /// <summary>
        /// 赋值表的主键、主键名称、记录数
        /// </summary>
        /// <param name="list"></param>
        private async void SetTableDetail(IEnumerable<TableInfo> list)
        {
            List<TableInfo> detailList = await GetTableDetailList();
            foreach (TableInfo table in list)
            {
                table.TableKey = string.Join(",", detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableKey));
                table.TableKeyName = detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableKeyName).FirstOrDefault();
                table.TableCount = detailList.Where(p => p.TableName == table.TableName).Select(p => p.TableCount).FirstOrDefault();
            }
        }
        private string GetDatabase()
        {
            string database = HtmlHelper.Resove(GlobalContext.Configuration.GetSection("DB:ConnectionString").Value.ToLower(), "database=", ";");
            return database;
        }
        #endregion
    }
}
