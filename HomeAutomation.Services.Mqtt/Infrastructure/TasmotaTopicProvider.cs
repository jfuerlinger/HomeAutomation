using HomeAutomation.Core.Model;
using MQTTnet;
using System.Text;

namespace HomeAutomation.Services.Mqtt.Infrastructure
{
    class TasmotaMqttMessageProcessor
    {
        private readonly List<string> _deviceTopics = new();

        public bool ProcessStatusChanges { get; private set; } = true;
        public bool ListenToPowerMessages { get; private set; } = true;
        public bool ListenToOverallStatusMessages { get; private set; } = false;

        public TasmotaMqttMessageProcessor WithDeviceTopic(string deviceTopic)
        {
            _deviceTopics.Add(deviceTopic);
            return this;
        }

        public TasmotaMqttMessageProcessor AddDeviceTopic(IEnumerable<string> deviceTopics)
        {
            _deviceTopics.AddRange(deviceTopics);
            return this;
        }

        public TasmotaMqttMessageProcessor WithStatusChangeProcessing(bool processStatusChanges)
        {
            ProcessStatusChanges = processStatusChanges;
            return this;
        }

        public TasmotaMqttMessageProcessor WithPowerMessageListening(bool listenToPowerMessages)
        {
            ListenToPowerMessages = listenToPowerMessages;
            return this;
        }

        public TasmotaMqttMessageProcessor WithOverallStatusMessageListening(bool listenToOverallStatusMessages)
        {
            ListenToOverallStatusMessages = listenToOverallStatusMessages;
            return this;
        }

        private static string ExtractDeviceTopic(string messageTopic)
            => messageTopic
                .Replace("stat/", string.Empty)
                .Replace("/POWER", string.Empty)
                .Replace("/RESULT", string.Empty);

        public void ProcessMessage(MqttApplicationMessage message)
        {
            string messageTopic = message.Topic;
            string deviceTopic = ExtractDeviceTopic(messageTopic);
            string payload = Encoding.UTF8.GetString(message.Payload);

            if (ProcessStatusChanges && messageTopic.StartsWith("stat/"))
            {
                if (ListenToPowerMessages && messageTopic.EndsWith("/POWER"))
                {
                    OnDeviceStateChanged(deviceTopic, payload);
                }

                if (ListenToOverallStatusMessages && messageTopic.EndsWith("/RESULT"))
                {
                    OnDeviceStateChanged(deviceTopic, payload);
                }
            }
        }

        public event EventHandler<DeviceStateChanged>? DeviceStateChanged;

        protected virtual void OnDeviceStateChanged(string topic, string state)
        {
            DeviceStateChanged?.Invoke(this, new DeviceStateChanged(topic, state));
        }

        internal string BuildPowerRequestStatusMessage(string deviceTopic) => $"cmnd/{deviceTopic}/POWER";
    }
}
