using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSMS.Entity.SystemManage
{
    [Table("sys_auto_job_log")]
    public class AutoJobLogEntity : BaseCreateEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string JobGroupName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string JobName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int? LogStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Remark { get; set; }
    }
}
