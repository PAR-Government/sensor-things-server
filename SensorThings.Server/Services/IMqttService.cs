using System.Threading.Tasks;
using MQTTnet.Server;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IMqttService
    {
        public Task PublishObservationAsync(Observation observation);
        public void Configure(IMqttServer mqtt);
    }
}