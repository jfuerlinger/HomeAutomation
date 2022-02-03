using HomeAutomation.Core.Contracts.Services;
using HomeAutomation.Core.Model;
using HomeAutomation.Services.Mqtt.Infrastructure;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using System.Text;

namespace HomeAutomation.Services.Mqtt
{
    public class MqttService : IMqttService
    {
        private readonly IMqttClient _mqttClient;
        private readonly List<IObserver<DeviceStateChanged>> _observers;
        private readonly ILogger<MqttService> _logger;
        private bool _disposedValue;

        public MqttService(ILogger<MqttService> logger)
        {
            _logger = logger;
            _mqttClient = new MqttFactory().CreateMqttClient();
            _observers = new List<IObserver<DeviceStateChanged>>();

            var mqttMessageProcessor = new TasmotaMqttMessageProcessor()
                    .WithStatusChangeProcessing(true)
                    .WithOverallStatusMessageListening(false)
                    .WithPowerMessageListening(true);
            mqttMessageProcessor.DeviceStateChanged += (object? sender, DeviceStateChanged e) => _observers.ForEach(observer => observer.OnNext(e));

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                StringBuilder sb = new();

                sb.AppendLine("### RECEIVED APPLICATION MESSAGE ###");
                sb.AppendLine($"+ Topic = {e.ApplicationMessage.Topic}");
                sb.AppendLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                sb.AppendLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                sb.AppendLine($"+ Retain = {e.ApplicationMessage.Retain}");
                string message = sb.ToString();
                _logger.LogInformation(message);

                mqttMessageProcessor.ProcessMessage(e.ApplicationMessage);
            });
        }

        public async Task InitAsync(CancellationToken cancellationToken)
        {
            await ConnectAsync(cancellationToken);

            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(
                    new MqttTopicFilterBuilder().WithTopic("stat/#")
                        .Build())
                .Build());
        }

        public async ValueTask DisposeAsync()
        {
            await _mqttClient.DisconnectAsync();
        }

        public async Task PublishAsync(
            string topic, string command,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));
            ArgumentNullException.ThrowIfNull(command, nameof(command));

            if (!_mqttClient.IsConnected)
            {
                await ConnectAsync(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(command)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build(), cancellationToken);
            }
        }

        public IDisposable Subscribe(IObserver<DeviceStateChanged> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer);
        }


        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await _mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
                                            .WithTcpServer("192.168.1.201", 1883)
                                            .Build(),
                                          cancellationToken);
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<DeviceStateChanged>> _observers;
            private readonly IObserver<DeviceStateChanged> _observer;

            public Unsubscriber(
                List<IObserver<DeviceStateChanged>> observers,
                IObserver<DeviceStateChanged> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mqttClient.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
