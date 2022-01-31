using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface ISensorsService
    {
        Task<Sensor> AddSensor(Sensor sensor);
        Task<IEnumerable<Datastream>> GetLinkedDatastreamsAsync(long sensorId);
        Task<Sensor> GetSensorById(int id);
        Task<IEnumerable<Sensor>> GetSensors();
        Task RemoveSensor(int id);
        Task<Sensor> UpdateSensor(JObject updates, int id);
    }
}