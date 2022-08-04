using System;
using System.Data;
using System.Globalization;
using static Dapper.SqlMapper;

namespace SensorThings.Server
{
    public class DapperDateTimeHandler : TypeHandler<DateTime>
    {
        private static readonly string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        private DapperDateTimeHandler() { }
        public static DapperDateTimeHandler Instance { get; } = new DapperDateTimeHandler();

        public override DateTime Parse(object value)
        {
            string strValue = (string)value;

            return DateTime.Parse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            string v = value.ToString(dateFormat);
            parameter.Value = v;
        }
    }
}

