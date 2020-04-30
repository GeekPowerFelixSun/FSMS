using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Model.Param.OrganizationManage
{
    public class PositionListParam
    {
        public string PositionName { get; set; }
        /// <summary>
        /// 多个职位Id
        /// </summary>
        public string PositionIds { get; set; }
    }
}
