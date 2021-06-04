using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ObservationsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public ObservationsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<Observation> AddObservation(Observation observation)
        {
            throw new NotImplementedException();
        }

        public async Task<Observation> GetObservationById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Observation>> GetObservations()
        {
            throw new NotImplementedException();
        }

        public async Task<Observation> UpdateObservation(JObject updates, int id)
        {
            throw new NotImplementedException();

        }

        public async Task RemoveObservation(int id)
        {
            throw new NotImplementedException();
        }
    }
}
