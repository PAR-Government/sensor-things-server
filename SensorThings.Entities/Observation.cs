using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorThings.Entities
{
    public class Observation : BaseEntity
    {
        [JsonProperty("phenomenonTime")]
        public OGCTime PhenomenonTime { get; set; }

        [JsonProperty("resultTime")]
        public DateTime ResultTime { get; set; }

        [JsonProperty("result")]
        public JObject Result { get; set; }

        // TODO: ResultQuality is not implemented

        [JsonProperty("validTime")]
        public OGCTime ValidTime { get; set; }

        [JsonProperty("parameters")]
        public JObject Parameters { get; set; }

        public override string SelfLink { get => $"{BaseUrl}/Observations({ID})"; }

        [JsonProperty("Datastream@iot.navigationLink")]
        public string DatastreamLink { get => $"{SelfLink}/Datastream"; }

        [JsonProperty("FeatureOfInterest@iot.navigationLink")]
        public string FeatureOfInterestLink { get => $"{SelfLink}/FeatureOfInterest"; }
    }
}
