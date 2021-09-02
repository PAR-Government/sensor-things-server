using Moq;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using SensorThings.Server.Test.TestObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SensorThings.Server.Test
{
    public class ObservationsServiceTest
    {
        [Fact]
        public async Task Test_AddObservation()
        {
            var resultObject = new JObject
            {
                { "value", 42 }
            };
            var obsId = 42;
            Observation observation = new Observation 
            { 
                ID = obsId, 
                Result = resultObject
            };

            Mock<IDatastreamsRepository> dsMockRepo = new Mock<IDatastreamsRepository>();
            Mock<IObservationsRepository> obsRepoMock = new Mock<IObservationsRepository>();
            var repoFactory = new TestRepoFactory 
            {
                ObservationsRepository = obsRepoMock.Object,
                DatastreamsRepository = dsMockRepo.Object
            };
            var service = new ObservationsService(repoFactory.CreateUnitOfWork());

            var createdObs = await service.AddObservation(observation, 1);

            obsRepoMock.Verify(m => m.AddAsync(createdObs));
        }

        [Fact]
        public async Task Test_GetObservationById()
        {
            var resultObject = new JObject
            {
                { "value", 42 }
            };
            int id = 1;
            Observation observation = new Observation { Result = resultObject };
            Mock<IObservationsRepository> obsRepoMock = new Mock<IObservationsRepository>();
            obsRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(observation);
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory.CreateUnitOfWork());

            var recoveredObs = await service.GetObservationById(id);

            obsRepoMock.Verify(m => m.GetByIdAsync(id));
            Assert.NotNull(recoveredObs);
        }

        [Fact]
        public async Task Test_GetObservations()
        {
            Observation observation1 = new Observation { ID = 1 };
            Observation observation2 = new Observation { ID = 2 };
            var observationCollection = new List<Observation> { observation1, observation2 };
            Mock<IObservationsRepository> obsRepoMock = new Mock<IObservationsRepository>();
            obsRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(observationCollection);
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory.CreateUnitOfWork());

            var observations = await service.GetObservations();

            obsRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(observations);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            var resultObject = new JObject
            {
                { "value", 42 }
            };
            int id = 42;
            var updates = JObject.Parse("{\"Result\": {\"value\": 24}}");
            Observation observation1 = new Observation { ID = 1, Result = resultObject };
            Mock<IObservationsRepository> obsRepoMock = new Mock<IObservationsRepository>();
            obsRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(observation1);
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory.CreateUnitOfWork());

            var updatedObservation = await service.UpdateObservation(updates, id);

            obsRepoMock.Verify(m => m.UpdateAsync(updatedObservation));
            Assert.Equal(24, updatedObservation.Result.GetValue("value").ToObject<int>());
        }

        [Fact]
        public async Task Test_RemoveObservation()
        {
            int id = 1;
            Mock<IObservationsRepository> obsRepoMock = new Mock<IObservationsRepository>();
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory.CreateUnitOfWork());

            await service.RemoveObservation(id);

            obsRepoMock.Verify(m => m.Remove(id));
        }
    }
}
