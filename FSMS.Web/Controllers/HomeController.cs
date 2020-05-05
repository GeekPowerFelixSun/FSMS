﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using FSMS.Business.OrganizationManage;
using FSMS.Business.SystemManage;
using FSMS.Entity.OrganizationManage;
using FSMS.Entity.SystemManage;
using FSMS.Enum;
using FSMS.Model.Result;
using FSMS.Util;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using FSMS.Web.Code;
using FSMS.Web.Handle;

namespace FSMS.Web.Controllers
{
    public class HomeController:BaseController
    {
        private MenuBLL baseMenuBLL = new MenuBLL();    //菜单
        private UserBLL sysUserBLL = new UserBLL();     //user
        private LogLoginBLL logLoginBLL = new LogLoginBLL();    //login

        #region 视图功能
        [HttpGet]
        [AuthorizeFilter]

        public async Task<IActionResult> Index()
        {
            OperatorInfo operatorInfo = await Operator.Instance.Current();

            TData<List<MenuEntity>> objMenu = await baseMenuBLL.GetList(null);
            List<MenuEntity> menuList = objMenu.Result;
            menuList = menuList.Where(p => p.MenuStatus == StatusEnum.Yes.ParseToInt()).ToList();

            if (operatorInfo.IsSystem != 1)
            {
                TData<List<MenuAuthorizeInfo>> objMenuAuthorize = await new MenuAuthorizeBLL().GetAuthorizeList(operatorInfo);
                List<long?> authorizeMenuIdList = objMenuAuthorize.Result.Select(p => p.MenuId).ToList();
                menuList = menuList.Where(p => authorizeMenuIdList.Contains(p.Id)).ToList();
            }

            ViewBag.MenuList = menuList;
            ViewBag.OperatorInfo = operatorInfo;
            return View();
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (GlobalContext.SystemConfig.Demo)
            {
                ViewBag.UserName = "admin";
                ViewBag.Password = "123456";
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoginOff()
        {
            #region 退出系统
            OperatorInfo user = await Operator.Instance.Current();
            if (user != null)
            {
                // 如果不允许同一个用户多次登录，当用户登出的时候，就不在线了
                if (!GlobalContext.SystemConfig.LoginMultiple)
                {
                    await new UserBLL().UpdateUser(new UserEntity { Id = user.UserId, IsOnline = 0 });
                }

                // 登出日志
                await logLoginBLL.SaveForm(new LogLoginEntity
                {
                    LogStatus = OperateStatusEnum.Success.ParseToInt(),
                    Remark = "退出系统",
                    IpAddress = NetHelper.Ip,
                    IpLocation = string.Empty,
                    Browser = NetHelper.Browser,
                    OS = NetHelper.GetOSVersion(),
                    ExtraRemark = NetHelper.UserAgent,
                    BaseCreatorId = user.UserId
                });

                Operator.Instance.RemoveCurrent();
                new CookieHelper().RemoveCookie("RememberMe");
            }
            #endregion
            return View(nameof(Login));         //登陆界面
        }

        [HttpGet]
        public IActionResult NoPermission()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public IActionResult Skin()
        {
            return View();
        }
        #endregion

        #region 获取数据
        public IActionResult GetCaptchaImage()
        {
            var test = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>().HttpContext.Session.Id;
            Tuple<string, int> captchaCode = CaptchaHelper.GetCaptchaCode();
            byte[] bytes = CaptchaHelper.CreateCaptchaImage(captchaCode.Item1);
            new SessionHelper().WriteSession("CaptchaCode", captchaCode.Item2);
            return File(bytes, @"image/jpeg");
        }
        #endregion

        #region 提交数据
        [HttpPost]
        public async Task<IActionResult> LoginJson(string userName, string password, string captchaCode)
        {
            TData obj = new TData();
            if (string.IsNullOrEmpty(captchaCode))
            {
                obj.Message = "验证码不能为空";
                return Json(obj);
            }
            if (captchaCode != new SessionHelper().GetSession("CaptchaCode").ParseToString())
            {
                obj.Message = "验证码错误，请重新输入";
                return Json(obj);
            }
            TData<UserEntity> userObj = await sysUserBLL.CheckLogin(userName, password, (int)PlatformEnum.Web);

            if (userObj.Tag == 1)
            {
                try
                {
                    await new UserBLL().UpdateUser(userObj.Result);
                    await Operator.Instance.AddCurrent(userObj.Result.WebToken);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }

            string ip = NetHelper.Ip;
            string browser = NetHelper.Browser;
            string os = NetHelper.GetOSVersion();
            string userAgent = NetHelper.UserAgent;

            Action taskAction = async () =>
            {
                LogLoginEntity logLoginEntity = new LogLoginEntity
                {
                    LogStatus = userObj.Tag == 1 ? OperateStatusEnum.Success.ParseToInt() : OperateStatusEnum.Fail.ParseToInt(),
                    Remark = userObj.Message,
                    IpAddress = ip,
                    IpLocation = IpLocationHelper.GetIpLocation(ip),
                    Browser = browser,
                    OS = os,
                    ExtraRemark = userAgent,
                    BaseCreatorId = userObj.Result?.Id
                };

                // 让底层不用获取HttpContext
                logLoginEntity.BaseCreatorId = logLoginEntity.BaseCreatorId ?? 0;

                await new LogLoginBLL().SaveForm(logLoginEntity);
            };
            AsyncTaskHelper.StartTask(taskAction);

            obj.Tag = userObj.Tag;
            obj.Message = userObj.Message;
            return Json(obj);
        }
        #endregion
    }
}
