using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SensorThings.Entities
{
    public class Listing<T>
    {
        [JsonProperty("@iot.count")]
        public int Count { get => Items.Count; }

        [JsonProperty("value")]
        public List<T> Items { get; set; }
    }
}
