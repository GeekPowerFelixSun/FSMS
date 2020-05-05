﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using FSMS.Web.Code;
using FSMS.Util;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using FSMS.Entity.SystemManage;
using FSMS.Enum;
using FSMS.Business.SystemManage;

namespace FSMS.Web.Controllers
{
    /// <summary>
    /// 00.基础控制器，用来记录访问日志
    /// 01.Operator是对OperatorInfo的具体操作信息
    /// 02.TData 数据传输对象 -->来自于FSMS.Util.Model
    /// 03.NetHelper
    /// 04.LogOperateEntity
    /// 05.TextHelper 文本处理
    /// 06.OperateStatusEnum  操作状态
    /// 07.IpLocationHelper   IP位置查询，判断是否是外网IP
    /// 08.LogOperateBLL
    /// 09.AsyncTaskHelper  异步处理
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        ///  日志记录控制器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //开始写入日志记录
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string action = context.RouteData.Values["Action"].ParseToString();     //记录动作
            OperatorInfo user = await Operator.Instance.Current();                  //记录谁操作的

            if (GlobalContext.SystemConfig.Demo)
            {
                if (context.HttpContext.Request.Method.ToUpper() == "POST")
                {
                    if (action.ToUpper() != "LoginJson".ToUpper() && action.ToUpper() != "CodePreviewJson".ToUpper())
                    {
                        TData obj = new TData
                        {
                            Message = "演示模式，不允许操作"
                        };
                        context.Result = new CustomJsonResult { Value = obj };
                        return;
                    }
                }
            }

            var resultContext = await next();

            sw.Stop();
            string ip = NetHelper.Ip;
            LogOperateEntity operateEntity = new LogOperateEntity();
            var areaName = context.RouteData.DataTokens["area"] + "/";
            var controllerName = context.RouteData.Values["controller"] + "/";
            string currentUrl = "/" + areaName + controllerName + action;

            if (action.ParseToString().ToLower() != "GetServerJson".ToLower() && action.ParseToString().ToLower() != "Error".ToLower())
            {
                #region 获取请求参数
                switch (context.HttpContext.Request.Method.ToUpper())
                {
                    case "GET":
                        operateEntity.ExecuteParam = context.HttpContext.Request.QueryString.Value.ParseToString();
                        break;

                    case "POST":
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        foreach (var item in context.ActionDescriptor.Parameters)
                        {
                            var itemType = item.ParameterType;
                            if (itemType.IsClass && itemType.Name != "String")
                            {
                                PropertyInfo[] infos = itemType.GetProperties();
                                foreach (PropertyInfo info in infos)
                                {
                                    if (info.CanRead)
                                    {
                                        var propertyValue = context.HttpContext.Request.Form[info.Name];
                                        if (!param.ContainsKey(info.Name))
                                        {
                                            if (!string.IsNullOrEmpty(propertyValue))
                                            {
                                                param.Add(info.Name, propertyValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (param.Count > 0)
                        {
                            operateEntity.ExecuteUrl += context.HttpContext.Request.QueryString.Value.ParseToString();
                            operateEntity.ExecuteParam = TextHelper.GetSubString(JsonConvert.SerializeObject(param), 8000);
                        }
                        else
                        {
                            operateEntity.ExecuteParam = context.HttpContext.Request.QueryString.Value.ParseToString();
                        }
                        break;
                }
                #endregion

                #region 异常获取
                StringBuilder sbException = new StringBuilder();
                if (resultContext.Exception != null)
                {
                    Exception exception = resultContext.Exception;
                    sbException.AppendLine(exception.Message);
                    while (exception.InnerException != null)
                    {
                        sbException.AppendLine(exception.InnerException.Message);
                        exception = exception.InnerException;
                    }
                    sbException.AppendLine(resultContext.Exception.StackTrace);
                    operateEntity.LogStatus = OperateStatusEnum.Fail.ParseToInt();
                }
                else
                {
                    operateEntity.LogStatus = OperateStatusEnum.Success.ParseToInt();
                }
                #endregion

                #region 日志实体                  
                if (user != null)
                {
                    operateEntity.BaseCreatorId = user.UserId;
                }

                operateEntity.ExecuteTime = sw.ElapsedMilliseconds.ParseToInt();
                operateEntity.IpAddress = ip;
                operateEntity.ExecuteUrl = currentUrl.Replace("//", "/");
                operateEntity.ExecuteResult = TextHelper.GetSubString(sbException.ToString(), 4000);
                #endregion

                Action taskAction = async () =>
                {
                    // 让底层不用获取HttpContext
                    operateEntity.BaseCreatorId = operateEntity.BaseCreatorId ?? 0;

                    // 耗时的任务异步完成
                    operateEntity.IpLocation = IpLocationHelper.GetIpLocation(ip);
                    await new LogOperateBLL().SaveForm(operateEntity);
                };
                AsyncTaskHelper.StartTask(taskAction);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// 覆盖基类的Json方法，用来自定义序列化实体，比如把long类型转成字符串返回到前端
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public new CustomJsonResult Json(object data)
        {
            SetTDataMessage(data);

            return new CustomJsonResult
            {
                Value = data
            };
        }

        #region 根据action方法名赋值合适的message
        private void SetTDataMessage(object data)
        {
            string action = this.ControllerContext.RouteData.Values["Action"].ParseToString();
            TData obj = data as TData;
            if (obj != null && string.IsNullOrEmpty(obj.Message))
            {
                if (action.Contains("Delete"))
                {
                    obj.Message = "删除成功";
                }
                else if (action.Contains("Save"))
                {
                    obj.Message = "保存成功";
                }
                else
                {
                    obj.Message = "操作成功";
                }
            }
        }
        #endregion
    }

    public class CustomJsonResult : JsonResult
    {
        public CustomJsonResult() : base(string.Empty)
        { }

        public override void ExecuteResult(ActionContext context)
        {
            this.ContentType = "text/json;charset=utf-8;";

            JsonSerializerSettings jsonSerizlizerSetting = new JsonSerializerSettings();
            jsonSerizlizerSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            string json = JsonConvert.SerializeObject(Value, Formatting.None, jsonSerizlizerSetting);
            Value = json;
            base.ExecuteResult(context);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            this.ContentType = "text/json;charset=utf-8;";

            JsonSerializerSettings jsonSerizlizerSetting = new JsonSerializerSettings();
            jsonSerizlizerSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            string json = JsonConvert.SerializeObject(Value, Formatting.None, jsonSerizlizerSetting);
            Value = json.ToJObject();
            return base.ExecuteResultAsync(context);
        }
    }
}
