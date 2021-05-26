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
    public class DatastreamsServiceTest
    {
        [Fact]
        public async Task Test_AddDatastream()
        {
            Datastream datastream = new Datastream { Name = "Test Datastream" };
            Mock<IRepository<Datastream>> datastreamRepoMock = new Mock<IRepository<Datastream>>();
            var repoFactory = new TestRepoFactory { DatastreamsRepository = datastreamRepoMock.Object };
            var service = new DatastreamsService(repoFactory);

            var createdDatastream = await service.AddDatastream(datastream);

            datastreamRepoMock.Verify(m => m.AddAsync(createdDatastream));
        }
    }
}
