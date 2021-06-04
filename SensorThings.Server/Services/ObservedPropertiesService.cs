using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ObservedPropertiesService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public ObservedPropertiesService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<ObservedProperty> AddObservedProperty(ObservedProperty property)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.ObservedPropertiesRepository.AddAsync(property);
            uow.Commit();

            property.ID = id;

            return property;
        }

        public async Task<ObservedProperty> GetObservedPropertyById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ObservedPropertiesRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ObservedProperty>> GetObservedProperties()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ObservedPropertiesRepository.GetAllAsync();
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

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ObservedPropertiesRepository.UpdateAsync(observedProperty);
            uow.Commit();

            return observedProperty;
        }

        public async Task RemoveObservedProperty(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ObservedPropertiesRepository.Remove(id);
        }
    }
}
