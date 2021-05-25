using Moq;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using SensorThings.Server.Test.TestObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SensorThings.Server.Test
{
    public class ThingsServiceTest
    {
        [Fact]
        public async Task Test_GetThingById_NonExistentAsync()
        {
            int id = 0;
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Thing) null);
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var thing = await service.GetThingById(id);

            Assert.Null(thing);
        }

        [Fact]
        public async Task Test_GetThingById_ExistsAsync()
        {
            int id = 0;
            Thing testThing = new Thing { ID = id };
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(testThing);
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var recoveredThing = await service.GetThingById(id);

            Assert.NotNull(recoveredThing);
            Assert.Equal(testThing, recoveredThing);
        }

        [Fact]
        public async Task Test_AddThing()
        {
            Thing testThing = new Thing { Name = "Test Thing" };
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var recoveredThing = await service.AddThing(testThing);

            thingRepoMock.Verify(m => m.AddAsync(testThing));
        }

        [Fact]
        public async Task Test_GetThings()
        {
            var thingsCollection = new List<Thing> { new Thing { ID = 1 }, new Thing { ID = 2 }, new Thing { ID = 3 } };
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(thingsCollection);
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var things = await service.GetThings();

            thingRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(things);
        }

        [Fact]
        public async Task Test_RemoveThing()
        {
            int id = 42;
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            await service.RemoveThing(id);

            thingRepoMock.Verify(m => m.Remove(id));
        }

        [Fact]
        public async Task Test_Update_EmptyFields()
        {
            int id = 1;
            var updates = JObject.Parse("{}");
            var thing = new Thing { ID = 1, Name = "FOO" };
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(thing);
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var updatedThing = await service.UpdateThing(updates, id);

            Assert.Equal("FOO", updatedThing.Name);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 1;
            var updates = JObject.Parse("{\"Name\": \"FOO BAR\"}");
            var thing = new Thing { ID = 1, Name = "FOO" };
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(thing);
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var updatedThing = await service.UpdateThing(updates, id);

            Assert.Equal("FOO BAR", updatedThing.Name);
        }

        [Fact]
        public async Task Test_GetLocations_NoneAssociated()
        {
            int id = 42;
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetLinkedLocations(id)).ReturnsAsync(new List<Location>());
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var locations = await service.GetLocationsAsync(id);

            thingRepoMock.Verify(m => m.GetLinkedLocations(id));
            Assert.Empty(locations);
        }

        [Fact]
        public async Task Test_GetLocation_Associated()
        {
            var locations = new List<Location> { new Location { ID = 1 }, new Location { ID = 2 }, new Location { ID = 3 } };
            int id = 42;
            Mock<IThingsRepository> thingRepoMock = new Mock<IThingsRepository>();
            thingRepoMock.Setup(m => m.GetLinkedLocations(id)).ReturnsAsync(locations);
            var repoFactory = new TestRepoFactory { ThingsRepository = thingRepoMock.Object };
            var service = new ThingsService(repoFactory);

            var retrievedLocations = await service.GetLocationsAsync(id);

            thingRepoMock.Verify(m => m.GetLinkedLocations(id));
            Assert.NotEmpty(retrievedLocations);
        }
    }
}
