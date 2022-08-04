using SensorThings.Entities;
using System;
using System.Data;
using static Dapper.SqlMapper;

namespace SensorThings.Server
{
    class DapperOGCTimeHandler : TypeHandler<OGCTime>
    {
        private DapperOGCTimeHandler() { }
        public static DapperOGCTimeHandler Instance { get; } = new DapperOGCTimeHandler();

        public override OGCTime Parse(object value)
        {
            string strValue = (string)value;
            return OGCTime.FromString(strValue);
        }

        public override void SetValue(IDbDataParameter parameter, OGCTime value)
        {
            parameter.Value = value.ToString();
        }
    }
}
