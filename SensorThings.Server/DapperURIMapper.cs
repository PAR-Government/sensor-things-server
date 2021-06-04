using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Dapper.SqlMapper;

namespace SensorThings.Server
{
    public class DapperURIMapper : TypeHandler<Uri>
    {
        public static DapperURIMapper Instance { get; } = new DapperURIMapper();

        private DapperURIMapper() { }

        public override Uri Parse(object value)
        {
            var uri = (string)value;

            return uri == null ? null : new Uri(uri);
        }

        public override void SetValue(IDbDataParameter parameter, Uri value)
        {
            parameter.Value = value.ToString();
        }
    }
}
