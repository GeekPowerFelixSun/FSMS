using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSMS.Data.Repository;
using FSMS.Util;
using FSMS.Util.Extension;
using FSMS.Enum.OrganizationManage;

namespace FSMS.Web.Code
{
    /// <summary>
    /// 00.此处有调用ParseTostring(）--对数据的类型的转换，如果是转换失败，那么则忽略这样的报错
    /// 01.SecurityHelper 用MD5加密字符串，可选择生成16位或者32位的加密字符串 --来自于FSMS.Util.SecurityHelper
    /// 02.UserBelongTypeEnum
    /// 
    /// </summary>
    public class DataRepository : RepositoryFactory
    {
        public async Task<OperatorInfo> GetUserByToken(string token)
        {
            if (!SecurityHelper.IsSafeSqlParam(token))
            {
                return null;
            }
            token = token.ParseToString().Trim();
            //token = token.ToString().Trim();

            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  a.id as UserId,
                                    a.user_status as UserStatus,
                                    a.is_online as IsOnline,
                                    a.user_name as UserName,
                                    a.real_name as RealName,
                                    a.portrait as Portrait,
                                    a.department_id as DepartmentId,
                                    a.web_token as WebToken,
                                    a.api_token as ApiToken,
                                    a.is_system as IsSystem
                            FROM    sys_user a
                            WHERE   web_token = '" + token + "' or api_token = '" + token + "'  ");
            var operatorInfo = await BaseRepository().FindObject<OperatorInfo>(strSql.ToString());
            if (operatorInfo != null)
            {
                #region 角色
                strSql.Clear();
                strSql.Append(@"SELECT  a.belong_id as RoleId
                                FROM    sys_user_belong a
                                WHERE   a.user_id = " + operatorInfo.UserId + " AND");
                //此处是通过使用枚举法对角色进行定义UserBelongTypeEnum(),来自于04.Entity数据实体抽象层.FSMS.Enum.OrganizationManage.UserBelongTypeEnum
                strSql.Append("         a.belong_type = " + UserBelongTypeEnum.Role.ParseToInt());
                IEnumerable<RoleInfo> roleList = await BaseRepository().FindList<RoleInfo>(strSql.ToString());
                operatorInfo.RoleIds = string.Join(",", roleList.Select(p => p.RoleId).ToArray());
                #endregion

                #region 部门名称
                strSql.Clear();
                strSql.Append(@"SELECT  a.department_name
                                FROM    sys_department a
                                WHERE   a.id = " + operatorInfo.DepartmentId);
                object departmentName = await BaseRepository().FindObject(strSql.ToString());
                operatorInfo.DepartmentName = departmentName.ParseToString();
                //operatorInfo.DepartmentName = departmentName.ToString();
                #endregion
            }
            return operatorInfo;
        }

    }
}
