using FSMS.Util.Model;
using System;
using System.Threading.Tasks;

namespace FSMS.Business.AutoJob
{
    public interface IJobTask
    {
        Task<TData> Start();
    }
}
