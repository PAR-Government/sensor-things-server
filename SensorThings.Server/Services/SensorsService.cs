using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class SensorsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public SensorsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<Sensor> AddSensor(Sensor sensor)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.SensorsRepository.AddAsync(sensor);
            uow.Commit();

            sensor.ID = id;

            return sensor;
        }

        public async Task<Sensor> GetSensorById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.SensorsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Sensor>> GetSensors()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.SensorsRepository.GetAllAsync();
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

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.SensorsRepository.UpdateAsync(sensor);
            uow.Commit();

            return sensor;
        }

        public async Task RemoveSensor(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.SensorsRepository.Remove(id);
            uow.Commit();
        }
    }
}
