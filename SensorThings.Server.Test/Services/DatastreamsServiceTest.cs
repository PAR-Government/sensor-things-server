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
    public class DatastreamsServiceTest
    {
        [Fact]
        public async Task Test_AddDatastream()
        {
            Datastream datastream = new Datastream { Name = "Test Datastream" };
            Mock<IDatastreamsRepository> datastreamRepoMock = new Mock<IDatastreamsRepository>();
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory.CreateUnitOfWork());

            var createdDatastream = await service.AddDatastream(datastream);

            datastreamRepoMock.Verify(m => m.AddAsync(createdDatastream));
        }

        [Fact]
        public async Task Test_GetDatastreamById()
        {
            int id = 42;
            Datastream datastream = new Datastream { ID = id, Name = "Test Datastream" };
            Mock<IDatastreamsRepository> datastreamRepoMock = new Mock<IDatastreamsRepository>();
            datastreamRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(datastream);
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory.CreateUnitOfWork());

            var retrievedDatastream = await service.GetDatastreamById(id);

            Assert.NotNull(retrievedDatastream);
            Assert.Equal(datastream, retrievedDatastream);
        }

        [Fact]
        public async Task Test_GetDatastreams()
        {
            var datastreamCollection = new List<Datastream> 
            { 
                new Datastream { ID = 1, Name = "Test Datastream" } , 
                new Datastream { ID = 2, Name = "Test Datastream" } 
            };
            Mock<IDatastreamsRepository> datastreamRepoMock = new Mock<IDatastreamsRepository>();
            datastreamRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(datastreamCollection);
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory.CreateUnitOfWork());

            var datastreams = await service.GetDatastreams();

            datastreamRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(datastreams);
        }

        [Fact]
        public async Task Test_RemoveDatastream()
        {
            int id = 42;
            Mock<IDatastreamsRepository> datastreamRepoMock = new Mock<IDatastreamsRepository>();
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory.CreateUnitOfWork());

            await service.RemoveDatastream(id);

            datastreamRepoMock.Verify(m => m.Remove(id));
        }

        [Fact]
        public async Task Test_Update_EmptyFields()
        {
            var updates = JObject.Parse("{}");
            int id = 42;
            Datastream datastream = new Datastream { ID = id, Name = "Test Datastream" };
            Mock<IDatastreamsRepository> datastreamRepoMock = new Mock<IDatastreamsRepository>();
            datastreamRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(datastream);
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory.CreateUnitOfWork());

            var updatedDatastream = await service.UpdateDatastream(updates, id);

            Assert.Equal("Test Datastream", updatedDatastream.Name);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            var updates = JObject.Parse("{\"Name\": \"FOO BAR\"}");
            int id = 42;
            Datastream datastream = new Datastream { ID = id, Name = "Test Datastream" };
            Mock<IDatastreamsRepository> datastreamRepoMock = new Mock<IDatastreamsRepository>();
            datastreamRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(datastream);
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory.CreateUnitOfWork());

            var updatedDatastream = await service.UpdateDatastream(updates, id);

            Assert.Equal("FOO BAR", updatedDatastream.Name);
        }
    }
}
