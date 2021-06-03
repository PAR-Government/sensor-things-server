using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorThings.Entities
{
    public class Sensor : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("encodingType")]
        public string EncodingType { get; set; }

        [JsonProperty("metadata")]
        public JObject Metadata { get; set; }

        public override string SelfLink { get => $"{BaseUrl}/Sensors({ID})"; }

        [JsonProperty("Datastreams@iot.id")]
        public string DatastreamsLink { get => $"{SelfLink}/Datastreams"; }
    }
}
