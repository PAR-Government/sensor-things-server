using Moq;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using SensorThings.Server.Test.TestObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SensorThings.Server.Test
{
    public class SensorsServiceTest
    {
        [Fact]
        public async Task Test_AddSensor()
        {
            Sensor sensor = new Sensor { Name = "Test Sensor" };
            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            var createdSensor = await service.AddSensor(sensor);

            sensorRepoMock.Verify(m => m.AddAsync(createdSensor));
        }

        [Fact]
        public async Task Test_GetThingById_NoneExists()
        {
            int id = 42;
            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Sensor)null);
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            var sensor = await service.GetSensorById(id);

            Assert.Null(sensor);
            sensorRepoMock.Verify(m => m.GetByIdAsync(id));
        }

        [Fact]
        public async Task Test_GetThingById_Exists()
        {
            int id = 42;
            Sensor sensor = new Sensor { Name = "Test Sensor" };
            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(sensor);
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            var recoveredSensor = await service.GetSensorById(id);

            Assert.NotNull(sensor);
            sensorRepoMock.Verify(m => m.GetByIdAsync(id));
        }

        [Fact]
        public async Task Test_GetSensors()
        {
            var sensor1 = new Sensor { Name = "Test Sensor 1" };
            var sensor2 = new Sensor { Name = "Test Sensor 2" };
            var sensor3 = new Sensor { Name = "Test Sensor 3" };
            var collection = new List<Sensor> { sensor1, sensor2, sensor3 };

            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(collection);
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            var sensors = await service.GetSensors();

            Assert.NotEmpty(sensors);
            sensorRepoMock.Verify(m => m.GetAllAsync());
        }

        [Fact]
        public async Task Test_Update_EmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{}");
            Sensor sensor = new Sensor { Name = "Test Sensor" };
            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(sensor);
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            var updatedSensor = await service.UpdateSensor(updates, id);

            Assert.Equal("Test Sensor", sensor.Name);
            sensorRepoMock.Verify(m => m.UpdateAsync(updatedSensor));
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{\"Name\": \"FOO BAR\"}");
            Sensor sensor = new Sensor { Name = "Test Sensor" };
            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(sensor);
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            var updatedSensor = await service.UpdateSensor(updates, id);

            sensorRepoMock.Verify(m => m.UpdateAsync(updatedSensor));
            Assert.Equal("FOO BAR", updatedSensor.Name);
        }

        [Fact]
        public async Task Test_RemoveSensor()
        {
            int id = 42;
            Mock<IRepository<Sensor>> sensorRepoMock = new Mock<IRepository<Sensor>>();
            var repoFactory = new TestRepoFactory { SensorsRepository = sensorRepoMock.Object };
            var service = new SensorsService(repoFactory.CreateUnitOfWork());

            await service.RemoveSensor(id);

            sensorRepoMock.Verify(m => m.Remove(id));
        }
    }
}
