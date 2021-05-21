using System;
using Newtonsoft.Json;

namespace SensorThings.Entities
{
    public abstract class BaseEntity
    {
        [JsonProperty("@iot.id")]
        public long ID { get; set; }

        [JsonProperty("@iot.selfLink")]
        public abstract string SelfLink { get; }

        [JsonIgnore]
        public string BaseUrl { get; set; }
    }
}
