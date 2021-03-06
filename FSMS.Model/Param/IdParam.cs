﻿using FSMS.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Model.Param
{
    public class IdParam
    {
        /// <summary>
        /// 所有表的主键
        /// long返回到前端js的时候，会丢失精度，所以转成字符串
        /// </summary>
        [JsonConverter(typeof(StringJsonConverter))]
        public long? Id { get; set; }
    }
}
