using Moq;
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
    public class LocationsServiceTest
    {
        [Fact]
        public async Task Test_AddLocation()
        {
            Location location = new Location { Name = "Test Location" };
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory);

            var recoveredThing = await service.AddLocation(location);

            locationRepoMock.Verify(m => m.AddAsync(location));
        }

        [Fact]
        public async Task Test_GetThingById_NoneExist()
        {
            int id = 42;
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Location)null);
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory);

            var location = await service.GetLocationById(id);

            Assert.Null(location);
            locationRepoMock.Verify(m => m.GetByIdAsync(id));
        }

        [Fact]
        public async Task Test_GetThingById_Exists()
        {
            int id = 42;
            Location location = new Location { Name = "Test Location" };
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(location);
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory);

            var recoveredLocation = await service.GetLocationById(id);

            locationRepoMock.Verify(m => m.GetByIdAsync(id));
            Assert.NotNull(recoveredLocation);
        }

        [Fact]
        public async Task Test_GetLocations()
        {
            var locationCollection = new List<Location> { new Location { ID = 1 }, new Location { ID = 2 }, new Location { ID = 3 } };
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            locationRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(locationCollection);
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory);

            var locations = await service.GetLocations();

            locationRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(locations);
        }
    }
}
