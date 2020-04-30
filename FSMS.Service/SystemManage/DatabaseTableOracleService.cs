using FSMS.Model.Result.SystemManage;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Service.SystemManage
{
    public class DatabaseTableOracleService : IDatabaseTableService
    {
        public Task<bool> DatabaseBackup(string database, string backupPath)
        {
            throw new NotImplementedException();
        }

        public Task<List<TableFieldInfo>> GetTableFieldList(string tableName)
        {
            throw new NotImplementedException();
        }

        public Task<List<TableInfo>> GetTableList(string tableName)
        {
            throw new NotImplementedException();
        }

        public Task<List<TableInfo>> GetTablePageList(string tableName, Pagination pagination)
        {
            throw new NotImplementedException();
        }
    }
}
