using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class SensorsService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public SensorsService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<Sensor> AddSensor(Sensor sensor)
        {
            var id = await UOW.SensorsRepository.AddAsync(sensor);
            sensor.ID = id;

            return sensor;
        }

        public async Task<Sensor> GetSensorById(int id)
        {
            return await UOW.SensorsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Sensor>> GetSensors()
        {
            return await UOW.SensorsRepository.GetAllAsync();
        }

        public async Task<Sensor> UpdateSensor(JObject updates, int id)
        {
            var sensor = await GetSensorById(id);

            var sensorJson = JObject.FromObject(sensor);
            foreach (var property in updates.Properties())
            {
                sensorJson[property.Name] = property.Value;
            }

            sensor = sensorJson.ToObject<Sensor>();

            await UOW.SensorsRepository.UpdateAsync(sensor);

            return sensor;
        }

        public async Task RemoveSensor(int id)
        {
            await UOW.SensorsRepository.Remove(id);
        }

        public async Task<IEnumerable<Datastream>> GetLinkedDatastreamsAsync(long sensorId)
        {
            var datastreams = await UOW.DatastreamsRepository.GetLinkedDatastreamsForSensorAsync(sensorId);
            return datastreams;
        }
    }
}
