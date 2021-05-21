using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SensorThings.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValueCodes
    {
        [EnumMember(Value = "application/pdf")]
        PDF,

        [EnumMember(Value = "http://www.opengis.net/doc/IS/SensorML/2.0")]
        SensorML
    }
}
