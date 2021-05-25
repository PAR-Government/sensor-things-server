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
    }
}
