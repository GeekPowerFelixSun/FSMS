﻿using FSMS.Business.Cache;
using FSMS.Entity.SystemManage;
using FSMS.Enum.SystemManage;
using FSMS.Model.Result;
using FSMS.Util.Extension;
using FSMS.Util.Model;
using FSMS.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.SystemManage
{
    public class MenuAuthorizeBLL
    {
        private MenuAuthorizeCache menuAuthorizeCache = new MenuAuthorizeCache();
        private MenuCache menuCache = new MenuCache();

        #region 获取数据
        public async Task<TData<List<MenuAuthorizeInfo>>> GetAuthorizeList(OperatorInfo user)
        {
            TData<List<MenuAuthorizeInfo>> obj = new TData<List<MenuAuthorizeInfo>>();
            obj.Result = new List<MenuAuthorizeInfo>();

            List<MenuAuthorizeEntity> authorizeList = new List<MenuAuthorizeEntity>();
            List<MenuAuthorizeEntity> userAuthorizeList = null;
            List<MenuAuthorizeEntity> roleAuthorizeList = null;

            var menuAuthorizeCacheList = await menuAuthorizeCache.GetList();

            // 用户
            userAuthorizeList = menuAuthorizeCacheList.Where(p => p.AuthorizeId == user.UserId && p.AuthorizeType == AuthorizeTypeEnum.User.ParseToInt()).ToList();

            // 角色
            if (!string.IsNullOrEmpty(user.RoleIds))
            {
                List<long> roleIdList = user.RoleIds.Split(',').Select(p => long.Parse(p)).ToList();
                roleAuthorizeList = menuAuthorizeCacheList.Where(p => roleIdList.Contains(p.AuthorizeId.Value) && p.AuthorizeType == AuthorizeTypeEnum.Role.ParseToInt()).ToList();
            }

            // 排除重复的记录
            if (userAuthorizeList.Count > 0)
            {
                authorizeList.AddRange(userAuthorizeList);
                roleAuthorizeList = roleAuthorizeList.Where(p => !userAuthorizeList.Select(u => u.AuthorizeId).Contains(p.AuthorizeId)).ToList();
            }
            if (roleAuthorizeList != null && roleAuthorizeList.Count > 0)
            {
                authorizeList.AddRange(roleAuthorizeList);
            }

            List<MenuEntity> menuList = await menuCache.GetList();
            foreach (MenuAuthorizeEntity authorize in authorizeList)
            {
                obj.Result.Add(new MenuAuthorizeInfo
                {
                    MenuId = authorize.MenuId,
                    AuthorizeId = authorize.AuthorizeId,
                    AuthorizeType = authorize.AuthorizeType,
                    Authorize = menuList.Where(t => t.Id == authorize.MenuId).Select(t => t.Authorize).FirstOrDefault()
                });
            }
            obj.Tag = 1;
            return obj;
        }
        #endregion
    }
}
