using Newtonsoft.Json;
using SensorThings.Entities.JsonConverters;
using System;
using System.Globalization;

namespace SensorThings.Entities
{
    [JsonConverter(typeof(JsonOGCTimeConverter))]
    public class OGCTime
    {
        private static readonly string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }

        public OGCTime(DateTime start)
        {
            Start = start;
        }

        public OGCTime(DateTime start, DateTime stop)
        {
            Start = start;
            Stop = stop;
        }

        public override string ToString()
        {
            string start = Start.ToString(dateFormat);
            string stop = Stop.ToString(dateFormat);

            string intermediate;
            if (Stop != DateTime.MinValue)
            {
                intermediate = $"{start}/{stop}";
            }
            else
            {
                intermediate = $"{start}";
            }

            return intermediate;
        }

        public static OGCTime FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException();
            }

            // Split into date halves
            var dates = value.Split('/');

            DateTime start;
            DateTime stop = DateTime.MinValue;

            start = DateTime.Parse(dates[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            if (dates.Length == 2)
            {
                stop = DateTime.Parse(dates[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }

            return new OGCTime(start, stop);
        }
    }
}
