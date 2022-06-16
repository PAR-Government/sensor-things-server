using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorThings.Entities
{
    public class Thing : BaseEntity
    {
        // Mandatory
        [JsonProperty("name")]
        public string Name { get; set; }

        // Mandatory
        [JsonProperty("description")]
        public string Description { get; set; }

        // Zero to One
        private JObject _properties = new JObject();
        [JsonProperty("properties")]
        public JObject Properties 
        { 
            get => _properties; 
            set => _properties = value; 
        }

        public override string SelfLink { get => $"{BaseUrl}/Things({ID})"; }

        [JsonProperty("Datastreams@iot.navigationLink")]
        public string DatastreamsLink { get => $"{SelfLink}/Datastreams"; }

        [JsonProperty("HistoricalLocations@iot.navigationLink")]
        public string HistoricalLocationsLink { get => $"{SelfLink}/HistoricalLocations"; }

        [JsonProperty("Locations@iot.navigationLink")]
        public string LocationsLink { get => $"{SelfLink}/Locations"; }

        private List<Location> _locations = new List<Location>();
        public List<Location> Locations 
        { 
            get => _locations; 
            set => _locations = value; 
        }

        private List<Datastream> _datastreams = new List<Datastream>();
        public List<Datastream> Datastreams 
        { 
            get => _datastreams; 
            set => _datastreams = value; 
        }

        private List<HistoricalLocation> _historicalLocations = new List<HistoricalLocation>();
        public List<HistoricalLocation> HistoricalLocations 
        { 
            get => _historicalLocations; 
            set => _historicalLocations = value; 
        }
    }
}
