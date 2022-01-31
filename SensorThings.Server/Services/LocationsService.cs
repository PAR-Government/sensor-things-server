using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class LocationsService : ILocationsService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public LocationsService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<Location> AddLocation(Location location)
        {
            var id = await UOW.LocationsRepository.AddAsync(location);

            location.ID = id;

            return location;
        }

        public async Task<Location> GetLocationById(int id)
        {
            return await UOW.LocationsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Location>> GetLocations()
        {
            return await UOW.LocationsRepository.GetAllAsync();
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

            await UOW.LocationsRepository.UpdateAsync(location);

            return location;
        }

        public async Task RemoveLocation(int id)
        {
            await UOW.LocationsRepository.Remove(id);
        }
    }
}
