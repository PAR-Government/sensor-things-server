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
    public class LocationsServiceTest
    {
        [Fact]
        public async Task Test_AddLocation()
        {
            Location location = new Location { Name = "Test Location" };
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

            var createdLocation = await service.AddLocation(location);

            locationRepoMock.Verify(m => m.AddAsync(createdLocation));
        }

        [Fact]
        public async Task Test_GetThingById_NoneExist()
        {
            int id = 42;
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Location)null);
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

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
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

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
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

            var locations = await service.GetLocations();

            locationRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(locations);
        }

        [Fact]
        public async Task Test_Update_EmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{}");
            Location location = new Location { Name = "FOO" };
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(location);
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

            var updatedLocation = await service.UpdateLocation(updates, id);

            locationRepoMock.Verify(m => m.UpdateAsync(updatedLocation));
            Assert.Equal("FOO", updatedLocation.Name);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{\"Name\": \"FOO BAR\"}");
            Location location = new Location { Name = "FOO" };
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            locationRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(location);
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

            var updatedLocation = await service.UpdateLocation(updates, id);

            locationRepoMock.Verify(m => m.UpdateAsync(updatedLocation));
            Assert.Equal("FOO BAR", updatedLocation.Name);
        }

        [Fact]
        public async Task Test_RemoveLocation()
        {
            int id = 42;
            Mock<IRepository<Location>> locationRepoMock = new Mock<IRepository<Location>>();
            var repoFactory = new TestRepoFactory { LocationsRepository = locationRepoMock.Object };
            var service = new LocationsService(repoFactory.CreateUnitOfWork());

            await service.RemoveLocation(id);

            locationRepoMock.Verify(m => m.Remove(id));
        }
    }
}
