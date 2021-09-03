using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorThings.Entities
{
    public class Datastream : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("observationType")]
        public string ObservationType { get; set; }

        [JsonProperty("unitOfMeasurement")]
        public string UnitOfMeasurement { get; set; }

        [JsonProperty("observedArea")]
        public JObject ObservedArea { get; set; }

        [JsonProperty("phenomenonTime")]
        public OGCTime PhenomenonTime { get; set; }

        [JsonProperty("resultTime")]
        public OGCTime ResultTime { get; set; }

        public override string SelfLink { get => $"{BaseUrl}/Datastreams({ID})"; }

        [JsonProperty("Observations@iot.navigationLink")]
        public string ObservationsLink { get => $"{SelfLink}/Observations"; }

        [JsonProperty("ObservedProperty@iot.navigationLink")]
        public string ObservedPropertyLink { get => $"{SelfLink}/ObservedProperty"; }

        [JsonProperty("Sensor@iot.navigationLink")]
        public string SensorLink { get => $"{SelfLink}/Sensor"; }

        [JsonProperty("Thing@iot.navigationLink")]
        public string ThingLink { get => $"{SelfLink}/Thing"; }

        [JsonProperty("Thing", NullValueHandling = NullValueHandling.Ignore)]
        public Thing Thing { get; set; }
    }
}
