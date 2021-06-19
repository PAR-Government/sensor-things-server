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
    public class ObservationsServiceTest
    {
        [Fact]
        public async Task Test_AddObservation()
        {
            Observation observation = new Observation { Result = JToken.FromObject(42) };
            Mock<IRepository<Observation>> obsRepoMock = new Mock<IRepository<Observation>>();
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory);

            var createdObs = await service.AddObservation(observation);

            obsRepoMock.Verify(m => m.AddAsync(createdObs));
        }

        [Fact]
        public async Task Test_GetObservationById()
        {
            int id = 1;
            Observation observation = new Observation { Result = JToken.FromObject(42) };
            Mock<IRepository<Observation>> obsRepoMock = new Mock<IRepository<Observation>>();
            obsRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(observation);
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory);

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
            Mock<IRepository<Observation>> obsRepoMock = new Mock<IRepository<Observation>>();
            obsRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(observationCollection);
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory);

            var observations = await service.GetObservations();

            obsRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(observations);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{\"Result\": 24}");
            Observation observation1 = new Observation { ID = 1, Result = JToken.FromObject(42) };
            Mock<IRepository<Observation>> obsRepoMock = new Mock<IRepository<Observation>>();
            obsRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(observation1);
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory);

            var updatedObservation = await service.UpdateObservation(updates, id);

            obsRepoMock.Verify(m => m.UpdateAsync(updatedObservation));
            Assert.Equal(24, updatedObservation.Result.ToObject<int>());
        }

        [Fact]
        public async Task Test_RemoveObservation()
        {
            int id = 1;
            Mock<IRepository<Observation>> obsRepoMock = new Mock<IRepository<Observation>>();
            var repoFactory = new TestRepoFactory { ObservationsRepository = obsRepoMock.Object };
            var service = new ObservationsService(repoFactory);

            await service.RemoveObservation(id);

            obsRepoMock.Verify(m => m.Remove(id));
        }
    }
}
