using FSMS.Model.Param.SystemManage;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Model.Param.OrganizationManage
{
    public class NewsListParam : BaseAreaParam
    {
        public string NewsTitle { get; set; }
        public int? NewsType { get; set; }
        public string NewsTag { get; set; }
    }
}
