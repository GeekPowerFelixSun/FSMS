using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using FSMS.Util;
using FSMS.Cache.Factory;

namespace FSMS.Web.Code
{
    public class Operator
    {
        //获取自定义属性本身，只读属性 Instance--实例
        public static Operator Instance { get { return new Operator(); } }

        //GlobalContext为全局方法，所有的服务注册以及注入，均在此处进行，包括这里用到的Configuration
        private string LoginProvider = GlobalContext.Configuration.GetSection("SystemConfig:LoginProvider").Value;
        private string TokenName = "UserToken"; //cookie name or session name

        public async Task AddCurrent(string token)
        {
            switch (LoginProvider)
            {
                //case "Cookie":
                //    new CookieHelper().RemoveCookie(TokenName);
                //    break;
                //case "Session":
                //    new SessionHelper().RemoveSession(TokenName);
                //    break;
                //case "WebApi":
                //    CacheFactory.Cache().RemoveCache(apiToken);
                //    break;
                case "Cookie":
                    new CookieHelper().WriteCookie(TokenName, token);
                    break;

                case "Session":
                    new SessionHelper().WriteSession(TokenName, token);
                    break;

                case "WebApi":
                    OperatorInfo user = await new DataRepository().GetUserByToken(token);
                    if (user != null)
                    {
                        CacheFactory.Cache().AddCache(token, user);
                    }
                    break;
                default:
                    throw new Exception("没有找到LogInProvider的配置。");
            }
        }

        /// <summary>
        /// Api接口需要传入apiToken
        /// </summary>
        /// <param name="apiToken"></param>
        public void RemoveCurrent(string apiToken = "")
        {
            switch (LoginProvider)
            {
                case "Cookie":
                    new CookieHelper().RemoveCookie(TokenName);
                    break;

                case "Session":
                    new SessionHelper().RemoveSession(TokenName);
                    break;

                case "WebApi":
                    CacheFactory.Cache().RemoveCache(apiToken);
                    break;

                default:
                    throw new Exception("未找到LoginProvider配置");
            }
        }

        /// <summary>
        /// Api接口需要传入apiToken
        /// </summary>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public async Task<OperatorInfo> Current(string apiToken = "")
        {
            IHttpContextAccessor hca = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>();
            OperatorInfo user = null;
            string token = string.Empty;
            switch (LoginProvider)
            {
                case "Cookie":
                    if (hca.HttpContext != null)
                    {
                        token = new CookieHelper().GetCookie(TokenName);
                    }
                    break;

                case "Session":
                    if (hca.HttpContext != null)
                    {
                        token = new SessionHelper().GetSession(TokenName);
                    }
                    break;

                case "WebApi":
                    token = apiToken;
                    break;
            }
            if (string.IsNullOrEmpty(token))
            {
                return user;
            }
            token = token.Trim('"');
            //从缓存容器中或许数据，判断是否引用缓存数据
            user = CacheFactory.Cache().GetCache<OperatorInfo>(token);
            if (user == null)
            {
                user = await new DataRepository().GetUserByToken(token);
                if (user != null)
                {
                    CacheFactory.Cache().AddCache(token, user);
                }
            }
            return user;
        }
    }
}
