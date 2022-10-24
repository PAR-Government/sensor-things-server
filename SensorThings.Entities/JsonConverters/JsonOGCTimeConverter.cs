using Newtonsoft.Json;
using System;
using System.Globalization;

namespace SensorThings.Entities.JsonConverters
{
    public class JsonOGCTimeConverter : JsonConverter<OGCTime>
    {
        public override OGCTime ReadJson(JsonReader reader, Type objectType, OGCTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Value is detected as a single DateTime
            var value = reader.Value;
            if (value is DateTime time) 
            {
                return new OGCTime(time, DateTime.MinValue);
            }
    
            string s = (string)reader.Value;

            if (s == null || string.IsNullOrWhiteSpace(s))
            {
                return new OGCTime(DateTime.MinValue, DateTime.MinValue);
            }

            // We have a string that should be a start/stop 
            // Split into date halves
            var dates = s.Split('/');

            var start = DateTime.ParseExact(dates[0], Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var stop = DateTime.ParseExact(dates[1], Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            return new OGCTime(start, stop);
        }

        public override void WriteJson(JsonWriter writer, OGCTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
