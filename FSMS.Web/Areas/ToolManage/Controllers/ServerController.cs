using FSMS.Util;
using FSMS.Util.Model;
using FSMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSMS.Web.Areas.ToolManage.Controllers
{
    [Area("ToolManage")]
    public class ServerController : BaseController
    {
        #region 视图功能
        public IActionResult ServerIndex()
        {
            return View();
        }
        #endregion

        #region 获取数据
        [HttpGet]
        public IActionResult GetServerJson()
        {
            TData<ComputerInfo> obj = new TData<ComputerInfo>();
            ComputerInfo computerInfo = null;
            try
            {
                computerInfo = ComputerHelper.GetComputerInfo();
                computerInfo.RunTime = DateTimeHelper.FormatTime(Environment.TickCount);
            }
            catch (Exception ex)
            {
                LogHelper.WriteWithTime(ex);
                obj.Message = ex.Message;
            }
            obj.Result = computerInfo;
            obj.Tag = 1;
            return Json(obj);
        }

        public IActionResult GetServerIpJson()
        {
            TData<string> obj = new TData<string>();
            string ip = NetHelper.GetWanIp();
            string ipLocation = IpLocationHelper.GetIpLocation(ip);
            obj.Result = string.Format("{0} ({1})", ip, ipLocation);
            obj.Tag = 1;
            return Json(obj);
        }
        #endregion

    }
}
