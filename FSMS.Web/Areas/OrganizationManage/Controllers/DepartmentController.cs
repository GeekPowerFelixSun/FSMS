using FSMS.Business.OrganizationManage;
using FSMS.Entity.OrganizationManage;
using FSMS.Model.Param.OrganizationManage;
using FSMS.Model.Result;
using FSMS.Util.Model;
using FSMS.Web.Controllers;
using FSMS.Web.Handle;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSMS.Web.Areas.OrganizationManage.Controllers
{
    [Area("OrganizationManage")]
    public class DepartmentController : BaseController
    {
        private DepartmentBLL sysDepartmentBLL = new DepartmentBLL();

        #region 视图功能
        [AuthorizeFilter("organization:department:view")]
        public IActionResult DepartmentIndex()
        {
            return View();
        }
        public IActionResult DepartmentForm()
        {
            return View();
        }
        #endregion

        #region 获取数据
        [HttpGet]
        [AuthorizeFilter("organization:department:search,organization:user:search")]
        public async Task<IActionResult> GetListJson(DepartmentListParam param)
        {
            TData<List<DepartmentEntity>> obj = await sysDepartmentBLL.GetList(param);
            return Json(obj);
        }

        [HttpGet]
        [AuthorizeFilter("organization:department:search,organization:user:search")]
        public async Task<IActionResult> GetDepartmentTreeListJson(DepartmentListParam param)
        {
            TData<List<ZtreeInfo>> obj = await sysDepartmentBLL.GetZtreeDepartmentList(param);
            return Json(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTreeListJson(DepartmentListParam param)
        {
            TData<List<ZtreeInfo>> obj = await sysDepartmentBLL.GetZtreeUserList(param);
            return Json(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetFormJson(long id)
        {
            TData<DepartmentEntity> obj = await sysDepartmentBLL.GetEntity(id);
            return Json(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetMaxSortJson()
        {
            TData<int> obj = await sysDepartmentBLL.GetMaxSort();
            return Json(obj);
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [AuthorizeFilter("organization:department:add,organization:department:edit")]
        public async Task<IActionResult> SaveFormJson(DepartmentEntity entity)
        {
            TData<string> obj = await sysDepartmentBLL.SaveForm(entity);
            return Json(obj);
        }

        [HttpPost]
        [AuthorizeFilter("organization:department:delete")]
        public async Task<IActionResult> DeleteFormJson(string ids)
        {
            TData obj = await sysDepartmentBLL.DeleteForm(ids);
            return Json(obj);
        }
        #endregion
    }
}
