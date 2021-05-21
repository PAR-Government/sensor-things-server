using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SensorThings.Server.Utils
{
    public static class ThingsSerializer
    {
        public static string Serialize(Thing thing, List<string> expansions)
        {
            JObject j = new JObject();
            j.Add("@iot.id", thing.ID);
            j.Add("Datastreams@iot.navigationLink", thing.DatastreamsLink);
            j.Add("HistoricalLocations@iot.navigationLink", thing.HistoricalLocationsLink);
            j.Add("Locations@iot.navigationLink", thing.LocationsLink);

            j.Add("name", thing.Name);
            j.Add("description", thing.Description);
            j.Add("properties", JToken.FromObject(thing.Properties));
            
            if (expansions != null) 
            {
                if (expansions.Contains(nameof(Thing.Locations)))
                {
                    j.Add("Locations@iot.count", thing.Locations.Count);
                    j.Add("Locations", JArray.FromObject(thing.Locations));
                }

                if (expansions.Contains(nameof(Thing.Datastreams)))
                {
                    j.Add("Datastreams@iot.count", thing.Datastreams.Count);
                    j.Add("Datastreams", JArray.FromObject(thing.Datastreams));
                }

                if (expansions.Contains(nameof(Thing.HistoricalLocations)))
                {
                    j.Add("HistoricalLocations@iot.count", thing.HistoricalLocations.Count);
                    j.Add("HistoricalLocations", JArray.FromObject(thing.HistoricalLocations));
                } 
            }

            return j.ToString();
        }
    }
}
