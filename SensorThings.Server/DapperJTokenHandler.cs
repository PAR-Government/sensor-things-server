using System;
using System.Data;
using Newtonsoft.Json.Linq;
using static Dapper.SqlMapper;

namespace SensorThings.Server
{
    ///// <summary>
    ///// Adapted from: https://github.com/DapperLib/Dapper/issues/719#issuecomment-284984074
    ///// </summary>
    public class DapperJTokenHandler: TypeHandler<JToken>
    {
        private DapperJTokenHandler() { }
        public static DapperJTokenHandler Instance { get; } = new DapperJTokenHandler();

        public override JToken Parse(object value)
        {
            var json = (string)value;
            return json == null ? null : JToken.Parse(json);
        }

        public override void SetValue(IDbDataParameter parameter, JToken value)
        {
            parameter.Value = value?.ToString(Newtonsoft.Json.Formatting.None);
        }
    }
}

