using System;
using Newtonsoft.Json;

namespace SensorThings.Entities
{
    public class HistoricalLocation : BaseEntity
    {
        [JsonProperty("time")]
        public DateTime Time { get; set; }

        public override string SelfLink { get => $"{BaseUrl}/HistoricalLocations({ID})"; }

        [JsonProperty("Locations@iot.navigationLink")]
        public string LocationsLink { get => $"{SelfLink}/Locations"; }

        [JsonProperty("Thing@iot.navigationLink")]
        public string ThingLink { get => $"{SelfLink}/Things"; }
    }
}
