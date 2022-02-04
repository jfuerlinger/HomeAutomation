using HomeAutomation.Core.Model;

namespace HomeAutomation.Core.Contracts.Services
{
    public interface IMqttService : IDisposable, IAsyncDisposable, IObservable<DeviceStateChanged>
    {
        Task PublishAsync(string topic, string payload, CancellationToken cancellationToken);
        Task InitAsync(CancellationToken cancellationToken);

        Task RequestPowerStateInfo(string deviceTopic, CancellationToken cancellationToken);
    }
}
