using FSMS.Util.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Model.Param.SystemManage
{
    public class DataDictListParam : BaseApiToken
    {
        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
