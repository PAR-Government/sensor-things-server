using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IDatastreamsRepository : IRepository<Datastream>
    {
        public Task LinkSensorAsync(long datastreamId, long sensorId);

        public Task<Sensor> GetLinkedSensorAsync(long datastreamId);

        public Task UnlinkSensorAsync(long datastreamId, long sensorId);

        public Task<IEnumerable<Datastream>> GetLinkedDatastreamsForSensorAsync(long sensorId);
    }
}
