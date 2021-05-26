using Newtonsoft.Json;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Dapper.SqlMapper;

namespace SensorThings.Server
{
    class DapperOGCTimeHandler : TypeHandler<OGCTime>
    {
        private DapperOGCTimeHandler() { }
        public static DapperOGCTimeHandler Instance { get; } = new DapperOGCTimeHandler();

        public override OGCTime Parse(object value)
        {
            var json = (string)value;
            return json == null ? null : JsonConvert.DeserializeObject<OGCTime>(json);
        }

        public override void SetValue(IDbDataParameter parameter, OGCTime value)
        {
            var s = JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None);
            parameter.Value = s;
        }
    }
}
