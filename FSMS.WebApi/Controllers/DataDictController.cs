using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSMS.Business.SystemManage;
using FSMS.Model.Param.SystemManage;
using FSMS.Model.Result.SystemManage;
using FSMS.Util.Model;
using FSMS.WebApi.Handle;
using Microsoft.AspNetCore.Mvc;

namespace FSMS.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [AuthorizeFilter]
    public class DataDictController : ControllerBase
    {
        private DataDictBLL dataDictBLL = new DataDictBLL();

        #region 获取数据
        /// <summary>
        /// 获取数据字典列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TData<List<DataDictInfo>>> GetList([FromQuery]DataDictListParam param)
        {
            TData<List<DataDictInfo>> obj = await dataDictBLL.GetDataDictList();
            obj.Tag = 1;
            return obj;
        }
        #endregion
    }
}
