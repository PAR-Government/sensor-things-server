using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IObservationsRepository : IRepository<Observation>
    {
        public Task LinkFeatureOfInterestAsync(long observationId, long featureId);

        public Task<FeatureOfInterest> GetLinkedFeatureOfInterestAsync(long observationId);

        public Task<IEnumerable<Observation>> GetLinkedObservationsForFeatureOfInterestAsync(long featureId);

        public Task UnlinkFeatureOfInterest(long observationId, long featureId);
    }
}
