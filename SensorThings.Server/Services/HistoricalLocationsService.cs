using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class HistoricalLocationsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public HistoricalLocationsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<HistoricalLocation> AddHistoricalLocation(HistoricalLocation location)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.HistoricalLocationsRepository.AddAsync(location);
            uow.Commit();

            location.ID = id;

            return location;
        }

        public async Task<HistoricalLocation> GetHistoricalLocationById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.HistoricalLocationsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<HistoricalLocation>> GetHistoricalLocations()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.HistoricalLocationsRepository.GetAllAsync();
        }

        public async Task<HistoricalLocation> UpdateHistoricalLocation(JObject updates, int id)
        {
            var location = await GetHistoricalLocationById(id);

            var locationJson = JObject.FromObject(location);
            foreach (var property in updates.Properties())
            {
                locationJson[property.Name] = property.Value;
            }

            location = locationJson.ToObject<HistoricalLocation>();

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.HistoricalLocationsRepository.UpdateAsync(location);
            uow.Commit();

            return location;
        }

        public async Task RemoveHistoricalLocation(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.HistoricalLocationsRepository.Remove(id);
            uow.Commit();
        }

        public async Task LinkLocation(long historicalLocationId, long locationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.HistoricalLocationsRepository.LinkLocationAsync(historicalLocationId, locationId);
            uow.Commit();
        }

        public async Task<IEnumerable<Location>> GetLinkedLocations(long id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.HistoricalLocationsRepository.GetLinkedLocations(id);
        }

        public async Task UnlinkLocation(long historicalLocationId, long locationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.HistoricalLocationsRepository.UnlinkLocationAsync(historicalLocationId, locationId);
            uow.Commit();
        }

        public async Task UnlinkLocations(long historicalLocationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.HistoricalLocationsRepository.UnlinkLocationsAsync(historicalLocationId);
            uow.Commit();
        }
    }
}
