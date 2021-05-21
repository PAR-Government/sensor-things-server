using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SensorThings.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObservationTypes
    {
        [EnumMember(Value = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_CategoryObservation")]
        OM_CategoryObservation,

        [EnumMember(Value = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_CountObservation")]
        OM_CountObservation,

        [EnumMember(Value = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement")]
        OM_Measurement,

        [EnumMember(Value = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Observation")]
        OM_Observation,

        [EnumMember(Value = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_TruthObservation")]
        OM_TruthObservation
    }
}
