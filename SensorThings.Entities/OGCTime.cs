using Newtonsoft.Json;
using SensorThings.Entities.JsonConverters;
using System;

namespace SensorThings.Entities
{
    [JsonConverter(typeof(JsonOGCTimeConverter))]
    public class OGCTime
    {
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
    }
}
