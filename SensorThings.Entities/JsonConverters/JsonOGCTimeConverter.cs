using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorThings.Entities.JsonConverters
{
    public class JsonOGCTimeConverter : JsonConverter<OGCTime>
    {
        public override OGCTime ReadJson(JsonReader reader, Type objectType, OGCTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            // Split into date halves
            var dates = s.Split('/');
            var start = DateTime.Parse(dates[0]);
            var stop = DateTime.Parse(dates[1]);

            return new OGCTime(start, stop);
        }

        public override void WriteJson(JsonWriter writer, OGCTime value, JsonSerializer serializer)
        {
            var start = value.Start.ToString("yyyyMMddTHH:mm:ssZ");
            var stop = value.Stop.ToString("yyyyMMddTHH:mm:ssZ");

            writer.WriteValue($"{start}/{stop}");
        }
    }
}
