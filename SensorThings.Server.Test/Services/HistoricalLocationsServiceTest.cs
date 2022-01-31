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

namespace SensorThings.Server.Test.Services
{
    public class HistoricalLocationsServiceTest
    {
        [Fact]
        public async Task Test_AddHistoricalLocation()
        {
            HistoricalLocation location = new HistoricalLocation();
            Mock<IHistoricalLocationsRepository> locationRepoMock = new Mock<IHistoricalLocationsRepository>();
            var repoFactory = new TestRepoFactory { HistoricalLocationsRepository = locationRepoMock.Object };
            var service = new HistoricalLocationsService(repoFactory.CreateUnitOfWork());

            var createdLocation = await service.AddHistoricalLocation(location);

            locationRepoMock.Verify(m => m.AddAsync(createdLocation));
        }

        [Fact]
        public async Task Test_GetLocationById()
        {
            int id = 1;
            HistoricalLocation location = new HistoricalLocation();
            Mock<IHistoricalLocationsRepository> locationRepoMock = new Mock<IHistoricalLocationsRepository>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(location);
            var repoFactory = new TestRepoFactory { HistoricalLocationsRepository = locationRepoMock.Object };
            var service = new HistoricalLocationsService(repoFactory.CreateUnitOfWork());

            var recoveredLocation = await service.GetHistoricalLocationById(id);

            locationRepoMock.Verify(m => m.GetByIdAsync(id));
            Assert.NotNull(recoveredLocation);
        }

        [Fact]
        public async Task Test_GetLocations()
        {
            HistoricalLocation location1 = new HistoricalLocation { ID = 1 };
            HistoricalLocation location2 = new HistoricalLocation { ID = 2 };
            var locationCollection = new List<HistoricalLocation> { location1, location2 };

            Mock<IHistoricalLocationsRepository> locationRepoMock = new Mock<IHistoricalLocationsRepository>();
            locationRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(locationCollection);
            var repoFactory = new TestRepoFactory { HistoricalLocationsRepository = locationRepoMock.Object };
            var service = new HistoricalLocationsService(repoFactory.CreateUnitOfWork());

            var locations = await service.GetHistoricalLocations();

            locationRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(locations);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 1;
            var time = DateTime.Now;
            var updatedTime = time.AddDays(2);
            var updates = new JObject();
            updates.Add("time", updatedTime);

            HistoricalLocation location = new HistoricalLocation { Time = time };
            Mock<IHistoricalLocationsRepository> locationRepoMock = new Mock<IHistoricalLocationsRepository>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(location);
            var repoFactory = new TestRepoFactory { HistoricalLocationsRepository = locationRepoMock.Object };
            var service = new HistoricalLocationsService(repoFactory.CreateUnitOfWork());

            var updatedLocation = await service.UpdateHistoricalLocation(updates, id);

            locationRepoMock.Verify(m => m.UpdateAsync(updatedLocation));
            Assert.Equal(updatedTime, updatedLocation.Time);
        }

        [Fact]
        public async Task Test_RemoveLocation()
        {
            int id = 1;
            Mock<IHistoricalLocationsRepository> locationRepoMock = new Mock<IHistoricalLocationsRepository>();
            var repoFactory = new TestRepoFactory { HistoricalLocationsRepository = locationRepoMock.Object };
            var service = new HistoricalLocationsService(repoFactory.CreateUnitOfWork());

            await service.RemoveHistoricalLocation(id);

            locationRepoMock.Verify(m => m.Remove(id));
        }
    }
}
