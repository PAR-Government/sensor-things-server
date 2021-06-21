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
    public class FeaturesOfInterestServiceTest
    {
        [Fact]
        public async Task Test_AddFeature()
        {
            FeatureOfInterest feature = new FeatureOfInterest { Name = "Test Feature" };
            Mock<IRepository<FeatureOfInterest>> featureRepoMock = new Mock<IRepository<FeatureOfInterest>>();
            var repoFactory = new TestRepoFactory { FeaturesOfInterestRepository = featureRepoMock.Object };
            var service = new FeaturesOfInterestService(repoFactory);

            var createdFeature = await service.AddFeature(feature);

            featureRepoMock.Verify(m => m.AddAsync(createdFeature));
        }

        [Fact]
        public async Task Test_GetFeatureById()
        {
            int id = 1;
            FeatureOfInterest feature = new FeatureOfInterest { Name = "Test Feature" };
            Mock<IRepository<FeatureOfInterest>> featureRepoMock = new Mock<IRepository<FeatureOfInterest>>();
            featureRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(feature);
            var repoFactory = new TestRepoFactory { FeaturesOfInterestRepository = featureRepoMock.Object };
            var service = new FeaturesOfInterestService(repoFactory);

            var recoveredFeature = await service.GetFeatureById(id);

            featureRepoMock.Verify(m => m.GetByIdAsync(id));
            Assert.NotNull(recoveredFeature);
        }

        [Fact]
        public async Task Test_GetFeatures()
        {
            FeatureOfInterest feature1 = new FeatureOfInterest { Name = "Test Feature 1" };
            FeatureOfInterest feature2 = new FeatureOfInterest { Name = "Test Feature 2" };
            var featureCollection = new List<FeatureOfInterest> { feature1, feature2 };

            Mock<IRepository<FeatureOfInterest>> featureRepoMock = new Mock<IRepository<FeatureOfInterest>>();
            featureRepoMock.Setup(m => m.GetAllAsync()).ReturnsAsync(featureCollection);
            var repoFactory = new TestRepoFactory { FeaturesOfInterestRepository = featureRepoMock.Object };
          
            var service = new FeaturesOfInterestService(repoFactory);

            var features = await service.GetFeatures();

            featureRepoMock.Verify(m => m.GetAllAsync());
            Assert.NotEmpty(features);
        }

        [Fact]
        public async Task Test_Update_NonEmptyFields()
        {
            int id = 1;
            var updates = JObject.Parse("{\"Name\": \"FOOBAR\"}");
            FeatureOfInterest feature = new FeatureOfInterest { Name = "Test Feature" };
            Mock<IRepository<FeatureOfInterest>> featureRepoMock = new Mock<IRepository<FeatureOfInterest>>();
            featureRepoMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(feature);
            var repoFactory = new TestRepoFactory { FeaturesOfInterestRepository = featureRepoMock.Object };
            var service = new FeaturesOfInterestService(repoFactory);

            var updatedFeature = await service.UpdateFeature(updates, id);

            featureRepoMock.Verify(m => m.UpdateAsync(updatedFeature));
            Assert.Equal("FOOBAR", updatedFeature.Name);
        }

        [Fact]
        public async Task Test_RemoveFeature()
        {
            int id = 1;
            Mock<IRepository<FeatureOfInterest>> featureRepoMock = new Mock<IRepository<FeatureOfInterest>>();
            var repoFactory = new TestRepoFactory { FeaturesOfInterestRepository = featureRepoMock.Object };
            var service = new FeaturesOfInterestService(repoFactory);

            await service.RemoveFeature(id);

            featureRepoMock.Verify(m => m.Remove(id));
        }
    }
}
