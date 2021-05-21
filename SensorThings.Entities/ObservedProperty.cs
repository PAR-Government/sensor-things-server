using System;
using Newtonsoft.Json;

namespace SensorThings.Entities
{
    public class ObservedProperty : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("definition")]
        public Uri Definition { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public override string SelfLink => throw new NotImplementedException();
    }
}
