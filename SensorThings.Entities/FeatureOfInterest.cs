using System;
using Newtonsoft.Json;

namespace SensorThings.Entities
{
    public class FeatureOfInterest : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("encodingType")]
        public ValueCodes EncodingType { get; set; }

        [JsonProperty("feature")]
        public Object Feature { get; set; }

        public override string SelfLink { get => $"{BaseUrl}/FeaturesOfInterest({ID})"; }

        [JsonProperty("Observations@iot.navigationLink")]
        public string ObservationsLink { get => $"{SelfLink}/Observations"; }
    }
}
