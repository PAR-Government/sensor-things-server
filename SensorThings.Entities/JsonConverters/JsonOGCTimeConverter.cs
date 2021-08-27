using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SensorThings.Entities.JsonConverters
{
    public class JsonOGCTimeConverter : JsonConverter<OGCTime>
    {
        private readonly string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public override OGCTime ReadJson(JsonReader reader, Type objectType, OGCTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
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

            // Split into date halves
            var dates = s.Split('/');

            var start = DateTime.Parse(dates[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var stop = DateTime.Parse(dates[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            return new OGCTime(start, stop);
        }

        public override void WriteJson(JsonWriter writer, OGCTime value, JsonSerializer serializer)
        {
            var start = value.Start.ToString(dateFormat);
            var stop = value.Stop.ToString(dateFormat);

            writer.WriteValue($"{start}/{stop}");
        }
    }
}
