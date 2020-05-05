﻿using FSMS.Business.SystemManage;
using FSMS.CodeGenerator;
using FSMS.CodeGenerator.Model;
using FSMS.CodeGenerator.Template;
using FSMS.Entity;
using FSMS.Model.Result;
using FSMS.Model.Result.SystemManage;
using FSMS.Util;
using FSMS.Util.Model;
using FSMS.Web.Code;
using FSMS.Web.Controllers;
using FSMS.Web.Handle;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FSMS.Web.Areas.ToolManage.Controllers
{
    [Area("ToolManage")]
    public class CodeGeneratorController : BaseController
    {
        private DatabaseTableBLL databaseTableBLL = new DatabaseTableBLL();

        #region 视图功能
        [AuthorizeFilter("tool:codegenerator:view")]
        public IActionResult CodeGeneratorIndex()
        {
            return View();
        }

        public IActionResult CodeGeneratorForm(string outputModule)
        {
            ViewBag.OutputModule = outputModule;
            return View();
        }

        public IActionResult CodeGeneratorEditSearch()
        {
            return View();
        }

        public IActionResult CodeGeneratorEditToolbar()
        {
            return View();
        }

        public IActionResult CodeGeneratorEditList()
        {
            return View();
        }

        #endregion

        #region 获取数据
        [HttpGet]
        [AuthorizeFilter("tool:codegenerator:search")]
        public async Task<IActionResult> GetTableFieldTreeListJson(string tableName)
        {
            TData<List<ZtreeInfo>> obj = await databaseTableBLL.GetTableFieldZtreeList(tableName);
            return Json(obj);
        }

        [HttpGet]
        [AuthorizeFilter("tool:codegenerator:search")]
        public async Task<IActionResult> GetTableFieldTreePartListJson(string tableName, int upper = 0)
        {
            TData<List<ZtreeInfo>> obj = await databaseTableBLL.GetTableFieldZtreeList(tableName);
            if (obj.Result != null)
            {
                // 基础字段不显示出来
                obj.Result.RemoveAll(p => BaseField.BaseFieldList.Contains(p.name));
                if (upper == 1)
                {
                    foreach (ZtreeInfo field in obj.Result)
                    {
                        field.name = TableMappingHelper.ConvertToUppercase(field.name);
                    }
                }
            }
            return Json(obj);
        }

        [HttpGet]
        [AuthorizeFilter("tool:codegenerator:search")]
        public async Task<IActionResult> GetBaseConfigJson(string tableName)
        {
            TData<BaseConfigModel> obj = new TData<BaseConfigModel>();

            string tableDescription = string.Empty;
            TData<List<TableFieldInfo>> tDataTableField = await databaseTableBLL.GetTableFieldList(tableName);
            List<string> columnList = tDataTableField.Result.Where(p => !BaseField.BaseFieldList.Contains(p.TableColumn)).Select(p => p.TableColumn).ToList();

            OperatorInfo operatorInfo = await Operator.Instance.Current();
            string serverPath = GlobalContext.HostingEnvironment.ContentRootPath;
            obj.Result = new SingleTableTemplate().GetBaseConfig(serverPath, operatorInfo.UserName, tableName, tableDescription, columnList);
            obj.Tag = 1;
            return Json(obj);
        }
        #endregion

        #region 提交数据
        [HttpPost]
        [AuthorizeFilter("tool:codegenerator:add")]
        public async Task<IActionResult> CodePreviewJson(BaseConfigModel baseConfig)
        {
            TData<object> obj = new TData<object>();
            if (string.IsNullOrEmpty(baseConfig.OutputConfig.OutputModule))
            {
                obj.Message = "请选择输出到的模块";
            }
            else
            {
                SingleTableTemplate template = new SingleTableTemplate();
                TData<List<TableFieldInfo>> objTable = await databaseTableBLL.GetTableFieldList(baseConfig.TableName);
                DataTable dt = DataHelper.ListToDataTable(objTable.Result);  // 用DataTable类型，避免依赖
                string codeEntity = template.BuildEntity(baseConfig, dt);
                string codeEntityParam = template.BuildEntityParam(baseConfig);
                string codeService = template.BuildService(baseConfig, dt);
                string codeBusiness = template.BuildBusiness(baseConfig);
                string codeController = template.BuildController(baseConfig);
                string codeIndex = template.BuildIndex(baseConfig);
                string codeForm = template.BuildForm(baseConfig);
                string codeMenu = template.BuildMenu(baseConfig);

                var json = new
                {
                    CodeEntity = HttpUtility.HtmlEncode(codeEntity),
                    CodeEntityParam = HttpUtility.HtmlEncode(codeEntityParam),
                    CodeService = HttpUtility.HtmlEncode(codeService),
                    CodeBusiness = HttpUtility.HtmlEncode(codeBusiness),
                    CodeController = HttpUtility.HtmlEncode(codeController),
                    CodeIndex = HttpUtility.HtmlEncode(codeIndex),
                    CodeForm = HttpUtility.HtmlEncode(codeForm),
                    CodeMenu = HttpUtility.HtmlEncode(codeMenu)
                };
                obj.Result = json;
                obj.Tag = 1;
            }
            return Json(obj);
        }

        [HttpPost]
        [AuthorizeFilter("tool:codegenerator:add")]
        public async Task<IActionResult> CodeGenerateJson(BaseConfigModel baseConfig, string Code)
        {
            TData<List<KeyValue>> obj = new TData<List<KeyValue>>();
            if (!GlobalContext.SystemConfig.Debug)
            {
                obj.Message = "请在本地开发模式时使用代码生成";
            }
            else
            {
                SingleTableTemplate template = new SingleTableTemplate();
                List<KeyValue> result = await template.CreateCode(baseConfig, HttpUtility.UrlDecode(Code));
                obj.Result = result;
                obj.Tag = 1;
            }
            return Json(obj);
        }
        #endregion
    }
}
