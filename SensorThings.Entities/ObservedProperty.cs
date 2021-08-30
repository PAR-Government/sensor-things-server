using System;
using System.Collections.Generic;
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

        public override string SelfLink { get => $"{BaseUrl}/ObservedProperties({ID})"; }

        public IList<Datastream> Datastreams { get; set; }

        [JsonProperty("Datastream@iot.navigationLink")]
        public string DatastreamsLink { get => $"{SelfLink}/Datastreams"; }
    }
}
