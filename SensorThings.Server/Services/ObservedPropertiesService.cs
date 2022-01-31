using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ObservedPropertiesService : IObservedPropertiesService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public ObservedPropertiesService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<ObservedProperty> AddObservedProperty(ObservedProperty property)
        {
            var id = await UOW.ObservedPropertiesRepository.AddAsync(property);
            property.ID = id;

            return property;
        }

        public async Task<ObservedProperty> GetObservedPropertyById(int id)
        {
            return await UOW.ObservedPropertiesRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ObservedProperty>> GetObservedProperties()
        {
            return await UOW.ObservedPropertiesRepository.GetAllAsync();
        }

        public async Task<ObservedProperty> UpdateObservedProperty(JObject updates, int id)
        {
            var observedProperty = await GetObservedPropertyById(id);

            var observedPropertyJson = JObject.FromObject(observedProperty);
            foreach (var property in updates.Properties())
            {
                observedPropertyJson[property.Name] = property.Value;
            }

            observedProperty = observedPropertyJson.ToObject<ObservedProperty>();

            await UOW.ObservedPropertiesRepository.UpdateAsync(observedProperty);

            return observedProperty;
        }

        public async Task RemoveObservedProperty(int id)
        {
            await UOW.ObservedPropertiesRepository.Remove(id);
        }

        public async Task<IEnumerable<Datastream>> GetLinkedDatastreams(long id)
        {
            var datastreams = await UOW.DatastreamsRepository.GetLinkedDatastreamsForObservedPropertyAsync(id);
            return datastreams;
        }
    }
}
