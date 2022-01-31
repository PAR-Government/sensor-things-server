using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IObservedPropertiesService
    {
        Task<ObservedProperty> AddObservedProperty(ObservedProperty property);
        Task<IEnumerable<Datastream>> GetLinkedDatastreams(long id);
        Task<IEnumerable<ObservedProperty>> GetObservedProperties();
        Task<ObservedProperty> GetObservedPropertyById(int id);
        Task RemoveObservedProperty(int id);
        Task<ObservedProperty> UpdateObservedProperty(JObject updates, int id);
    }
}