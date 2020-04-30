using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Util.Extension
{
    public static partial class Extensions
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }
    }
}
