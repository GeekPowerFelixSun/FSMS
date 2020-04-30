using FSMS.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Model.Result
{
    public class ZtreeInfo
    {
        [JsonConverter(typeof(StringJsonConverter))]
        public long? id { get; set; }

        [JsonConverter(typeof(StringJsonConverter))]
        public long? pId { get; set; }

        public string name { get; set; }
    }
}
