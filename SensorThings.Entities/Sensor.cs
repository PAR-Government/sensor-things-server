using System;
using Newtonsoft.Json;

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
        public Object Metadata { get; set; }

        public override string SelfLink => throw new NotImplementedException();
    }
}
