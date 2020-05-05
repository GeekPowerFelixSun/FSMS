﻿using FSMS.Business.SystemManage;
using FSMS.Entity.SystemManage;
using FSMS.Model.Param.SystemManage;
using FSMS.Util.Model;
using FSMS.Web.Controllers;
using FSMS.Web.Handle;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSMS.Web.Areas.SystemManage.Controllers
{
    [Area("SystemManage")]
    public class LogOperateController : BaseController
    {
        private LogOperateBLL sysLogOperateBLL = new LogOperateBLL();

        #region 视图功能
        [AuthorizeFilter("system:logoperate:view")]
        public IActionResult LogOperateIndex()
        {
            return View();
        }

        public IActionResult LogOperateDetail()
        {
            return View();
        }
        #endregion

        #region 获取数据
        [HttpGet]
        [AuthorizeFilter("system:logoperate:search")]
        public async Task<IActionResult> GetListJson(LogOperateListParam param)
        {
            TData<List<LogOperateEntity>> obj = await sysLogOperateBLL.GetList(param);
            return Json(obj);
        }

        [HttpGet]
        [AuthorizeFilter("system:logoperate:search")]
        public async Task<IActionResult> GetPageListJson(LogOperateListParam param, Pagination pagination)
        {
            TData<List<LogOperateEntity>> obj = await sysLogOperateBLL.GetPageList(param, pagination);
            return Json(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetFormJson(long id)
        {
            TData<LogOperateEntity> obj = await sysLogOperateBLL.GetEntity(id);
            return Json(obj);
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [AuthorizeFilter("system:logoperate:delete")]
        public async Task<IActionResult> DeleteFormJson(string ids)
        {
            TData obj = await sysLogOperateBLL.DeleteForm(ids);
            return Json(obj);
        }

        [HttpPost]
        [AuthorizeFilter("system:logoperate:delete")]
        public async Task<IActionResult> RemoveAllFormJson()
        {
            TData obj = await sysLogOperateBLL.RemoveAllForm();
            return Json(obj);
        }
        #endregion
    }
}
