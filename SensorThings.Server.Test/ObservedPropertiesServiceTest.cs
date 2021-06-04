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
    public class ObservedPropertiesServiceTest
    {
        [Fact]
        public async Task Test_AddObservedProperty()
        {
            var property = new ObservedProperty { Name = "Test ObservedProperty" };
            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            var createdProperty = await service.AddObservedProperty(property);

            propertyRepoMock.Verify(m => m.AddAsync(createdProperty));
        }

        [Fact]
        public async Task Test_GetPropertyById_NoneExists()
        {
            int id = 42;
            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            propertyRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((ObservedProperty)null);
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            var property = await service.GetObservedPropertyById(id);

            propertyRepoMock.Verify(m => m.GetByIdAsync(id));
        }

        [Fact]
        public async Task Test_GetPropertyById_Exists()
        {
            int id = 42;
            var property = new ObservedProperty { Name = "Property 1" };
            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            propertyRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(property);
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            var recoveredProperty = await service.GetObservedPropertyById(id);

            propertyRepoMock.Verify(m => m.GetByIdAsync(id));
            Assert.NotNull(recoveredProperty);
        }

        [Fact]
        public async Task Test_GetProperties()
        {
            var propertiesCollection = new List<ObservedProperty>() {
                new ObservedProperty { Name = "Property 1" },
                new ObservedProperty { Name = "Property 2" },
                new ObservedProperty { Name = "Property 3" } };

            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            propertyRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(propertiesCollection);
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            var properties = await service.GetObservedProperties();

            propertyRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(properties);
        }

        [Fact]
        public async Task Test_Update_EmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{}");
            var property = new ObservedProperty { Name = "Property 1" };
            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            propertyRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(property);
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            var recoveredProperty = await service.UpdateObservedProperty(updates, id);

            propertyRepoMock.Verify(m => m.UpdateAsync(recoveredProperty));
            Assert.Equal("Property 1", recoveredProperty.Name);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 42;
            var updates = JObject.Parse("{\"Name\": \"FOO BAR\"}");
            var property = new ObservedProperty { Name = "Property 1" };
            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            propertyRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(property);
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            var recoveredProperty = await service.UpdateObservedProperty(updates, id);

            propertyRepoMock.Verify(m => m.UpdateAsync(recoveredProperty));
            Assert.Equal("FOO BAR", recoveredProperty.Name);
        }

        [Fact]
        public async Task Test_RemoveProperty()
        {
            int id = 42;
            Mock<IRepository<ObservedProperty>> propertyRepoMock = new Mock<IRepository<ObservedProperty>>();
            var repoFactory = new TestRepoFactory { ObservedPropertiesRepository = propertyRepoMock.Object };
            var service = new ObservedPropertiesService(repoFactory);

            await service.RemoveObservedProperty(id);

            propertyRepoMock.Verify(m => m.Remove(id));
        }
    }
}
