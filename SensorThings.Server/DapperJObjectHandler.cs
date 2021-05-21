using System;
using System.Data;
using Newtonsoft.Json.Linq;
using static Dapper.SqlMapper;

namespace SensorThings.Server
{
    /// <summary>
    /// Adapted from: https://github.com/DapperLib/Dapper/issues/719#issuecomment-284984074
    /// </summary>
    public class DapperJObjectHandler : TypeHandler<JObject>
    {
            private DapperJObjectHandler() { }
            public static DapperJObjectHandler Instance { get; } = new DapperJObjectHandler();

            public override JObject Parse(object value)
            {
                var json = (string)value;
                return json == null ? null : JObject.Parse(json);
            }

            public override void SetValue(IDbDataParameter parameter, JObject value)
            {
                parameter.Value = value?.ToString(Newtonsoft.Json.Formatting.None);
            }
    }
}
