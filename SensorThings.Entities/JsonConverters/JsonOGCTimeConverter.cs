using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorThings.Entities.JsonConverters
{
    public class JsonOGCTimeConverter : JsonConverter<OGCTime>
    {
        private readonly string dateFormat = "yyyy-MM-ddTHH:mm:ssZ";

        public override OGCTime ReadJson(JsonReader reader, Type objectType, OGCTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            // Split into date halves
            var dates = s.Split('/');

            var start = DateTime.ParseExact(dates[0], dateFormat,
                System.Globalization.CultureInfo.InvariantCulture);
            var stop = DateTime.ParseExact(dates[1], dateFormat,
                System.Globalization.CultureInfo.InvariantCulture);

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
