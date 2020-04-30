using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Business.Cache
{
    interface IBusinessCache<T>
    {
        Task<List<T>> GetList();

        void Update(long id);

        void Remove(long id);
    }
}
