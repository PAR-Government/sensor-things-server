using MQTTnet.Client.Options;

namespace SensorThings.Server
{
    public interface IMqttClientOptionsFactory
    {
        IMqttClientOptions NewPublisherOptions();

        IMqttClientOptions NewSubscriberOptions();
    }
}
