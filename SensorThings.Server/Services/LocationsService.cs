using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class LocationsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public LocationsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<Location> AddLocation(Location location)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.LocationsRepository.AddAsync(location);
            uow.Commit();

            location.ID = id;

            return location;
        }

        public async Task<Location> GetLocationById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.LocationsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Location>> GetLocations()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.LocationsRepository.GetAllAsync();
        }

        public async Task<Location> UpdateLocation(JObject updates, int id)
        {
            var location = await GetLocationById(id);

            var locationJson = JObject.FromObject(location);
            foreach (var property in updates.Properties())
            {
                locationJson[property.Name] = property.Value;
            }

            location = locationJson.ToObject<Location>();

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.LocationsRepository.UpdateAsync(location);
            uow.Commit();

            return location;
        }

        public async Task RemoveLocation(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.LocationsRepository.Remove(id);
            uow.Commit();
        }
    }
}
