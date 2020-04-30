using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSMS.Entity.SystemManage;
using FSMS.Enum;
using FSMS.Model;
using FSMS.Model.Param.SystemManage;
using FSMS.Service.SystemManage;
using FSMS.Util.Model;
using FSMS.Util.Extension;
using System.Collections.Generic;
using FSMS.Entity.OrganizationManage;
using FSMS.Service.OrganizationManage;

namespace FSMS.Business.SystemManage
{
    /// <summary>
    /// 01.LogOperateService  获取数据 -->提交数据--> 私有方法
    /// 02.
    /// 03.
    /// 04.UserEntity 来自与实体
    /// 05.UserService
    /// 06.DepartmentEntity
    /// 07.DepartmentService
    /// </summary>
    public class LogOperateBLL
    {
        private LogOperateService logOperateService = new LogOperateService();

        #region 获取数据
        public async Task<TData<List<LogOperateEntity>>> GetList(LogOperateListParam param)
        {
            TData<List<LogOperateEntity>> obj = new TData<List<LogOperateEntity>>();
            obj.Result = await logOperateService.GetList(param);
            obj.TotalCount = obj.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<List<LogOperateEntity>>> GetPageList(LogOperateListParam param, Pagination pagination)
        {
            TData<List<LogOperateEntity>> obj = new TData<List<LogOperateEntity>>();
            obj.Result = await logOperateService.GetPageList(param, pagination);
            obj.TotalCount = pagination.TotalCount;
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<LogOperateEntity>> GetEntity(long id)
        {
            TData<LogOperateEntity> obj = new TData<LogOperateEntity>();
            obj.Result = await logOperateService.GetEntity(id);
            if (obj.Result != null)
            {
                UserEntity userEntity = await new UserService().GetEntity(obj.Result.BaseCreatorId.Value);
                if (userEntity != null)
                {
                    obj.Result.UserName = userEntity.UserName;
                    DepartmentEntity departmentEntitty = await new DepartmentService().GetEntity(userEntity.DepartmentId.Value);
                    if (departmentEntitty != null)
                    {
                        obj.Result.DepartmentName = departmentEntitty.DepartmentName;
                    }
                }
            }
            obj.Tag = 1;
            return obj;
        }
        #endregion

        #region 提交数据
        public async Task<TData<string>> SaveForm(LogOperateEntity entity)
        {
            TData<string> obj = new TData<string>();
            await logOperateService.SaveForm(entity);
            obj.Result = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData<string>> SaveForm(string remark)
        {
            TData<string> obj = new TData<string>();
            LogOperateEntity entity = new LogOperateEntity();
            await logOperateService.SaveForm(entity);
            entity.LogStatus = OperateStatusEnum.Success.ParseToInt();
            entity.ExecuteUrl = remark;
            obj.Result = entity.Id.ParseToString();
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> DeleteForm(string ids)
        {
            TData obj = new TData();
            await logOperateService.DeleteForm(ids);
            obj.Tag = 1;
            return obj;
        }

        public async Task<TData> RemoveAllForm()
        {
            TData obj = new TData();
            await logOperateService.RemoveAllForm();
            obj.Tag = 1;
            return obj;
        }
        #endregion
    }
}
