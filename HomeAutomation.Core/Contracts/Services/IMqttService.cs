using HomeAutomation.Core.Model;

namespace HomeAutomation.Core.Contracts.Services
{
    public interface IMqttService : IDisposable, IAsyncDisposable, IObservable<DeviceStateChanged>
    {
        Task PublishAsync(string topic, string command, CancellationToken cancellationToken);

        Task InitAsync(CancellationToken cancellationToken);
    }
}
