using System;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorThings.Entities
{
    public class Location : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("encodingType")]
        public string EncodingType { get; set; }

        [JsonProperty("location")]
        public JObject FeatureLocation { get; set; }

        public override string SelfLink { get => $"{BaseUrl}/Locations({ID})"; }

        [JsonProperty("Things@iot.navigationLink")]
        public string ThingsLink { get => $"{SelfLink}/Things"; }

        [JsonProperty("HistoricalLocations@iot.navigationLink")]
        public string HistoricalLocationsLink { get => $"{SelfLink}/HistoricalLocations"; }
    }
}
