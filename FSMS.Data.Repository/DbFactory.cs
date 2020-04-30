using FSMS.Data.EF;
using FSMS.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Data.Repository
{
    /// <summary>
    /// 日 期：2020.04.10
    /// 描 述：数据库建立工厂
    /// </summary>
    public class DbFactory
    {
        public static IDatabase Base()
        {
            IDatabase database = null;
            string dbType = GlobalContext.Configuration.GetSection("DB:DBType").Value;
            string dbConnectionString = GlobalContext.Configuration.GetSection("DB:ConnectionString").Value;
            switch (dbType)
            {
                case "SqlServer":
                    DbHelper.DbType = DatabaseType.SqlServer;
                    database = new SqlServerDatabase(dbConnectionString);
                    break;
                case "MySql":
                    DbHelper.DbType = DatabaseType.MySql;
                    database = new MySqlDatabase(dbConnectionString);
                    break;
                case "Oracle":
                    DbHelper.DbType = DatabaseType.Oracle;
                    //database = new OracleDatabase(sConnectString);
                    break;
                default:
                    throw new Exception("未找到数据库配置");
            }
            return database;
        }
    }
}
