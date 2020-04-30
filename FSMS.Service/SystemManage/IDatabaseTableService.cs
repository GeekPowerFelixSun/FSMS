using FSMS.Model.Result.SystemManage;
using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Service.SystemManage
{
    public interface IDatabaseTableService
    {
        Task<bool> DatabaseBackup(string database, string backupPath);
        Task<List<TableInfo>> GetTableList(string tableName);
        Task<List<TableInfo>> GetTablePageList(string tableName, Pagination pagination);
        Task<List<TableFieldInfo>> GetTableFieldList(string tableName);
    }
}
