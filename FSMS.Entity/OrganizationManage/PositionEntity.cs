using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FSMS.Entity.OrganizationManage
{
    [Table("sys_position")]
    public class PositionEntity : BaseExtensionEntity
    {
        public string PositionName { get; set; }
        public int? PositionSort { get; set; }
        public int? PositionStatus { get; set; }
        public string Remark { get; set; }
    }
}
