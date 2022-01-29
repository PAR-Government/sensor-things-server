using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SensorThings.Server.Utils;

namespace SensorThings.Server.Mqtt
{
    public class MqttRouter
    {
        private const string observationPattern = @"^(?<prefix>v1.0/)?Observations";
        private readonly Regex observationRegex = new Regex(observationPattern);

        private readonly IObservationsMqttController _observationsController;

        public MqttRouter(IObservationsMqttController observationsController)
        {
            _observationsController = observationsController;
        }

        public async Task<bool> Route(string topic, byte[] payload)
        {
            var isHandled = false;

            var match = observationRegex.Match(topic);

            if (match.Success)
            {
                isHandled = true;

                var stringPayload = PayloadUtils.ConvertUTF8BytesToString(payload);

                await _observationsController.Create(stringPayload);
            }

            return isHandled;
        }
    }
}
