using HomeAutomation.BlazorApp.Server.Hubs;
using HomeAutomation.Core.Contracts.Persistence;
using HomeAutomation.Core.Contracts.Services;
using HomeAutomation.Core.Model;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HomeAutomation.BlazorApp.Server.BackgroundServices
{
    public class HomeAutomationMonitor : BackgroundService, IObserver<DeviceStateChanged>
    {
        private readonly IMqttService _mqttService;
        private readonly IHubContext<HomeAutomationHub> _homeAutomationSignalRContext;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<HomeAutomationMonitor> _logger;
        private readonly List<string> _deviceTopicsToObserve;

        private readonly BlockingCollection<DeviceStateChanged> _deviceStateChanges;

        public HomeAutomationMonitor(
            IMqttService mqttService,
            IHubContext<HomeAutomationHub> homeAutomationSignalRContext,
            IDeviceRepository deviceRepository, 
            ILogger<HomeAutomationMonitor> logger)
        {
            _mqttService = mqttService;
            _homeAutomationSignalRContext = homeAutomationSignalRContext;
            _deviceRepository = deviceRepository;
            _logger = logger;

            _deviceTopicsToObserve = new() { "wohnung/buero/deckenlicht" };
            _deviceStateChanges = new();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(DeviceStateChanged value)
        {
            if (_deviceTopicsToObserve.Contains(value.DeviceTopic))
            {
                _logger.LogDebug($"DeviceTopic='{value.DeviceTopic}' State='{value.State}'");
                _deviceStateChanges.Add(new DeviceStateChanged(value.DeviceTopic, value.State));
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HomeAutomationMonitor is running ...");

            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(250));

            using var subscription = _mqttService.Subscribe(this);
            await _mqttService.InitAsync(stoppingToken);

            int executionCount = 0;
            do
            {
                _logger.LogDebug($"HomeAutomationMonitor is working. Count: {executionCount++}");

                while (_deviceStateChanges.TryTake(out DeviceStateChanged? item, TimeSpan.FromMilliseconds(100)))
                {
                    await NotifyClientsAsync(item);
                    
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task NotifyClientsAsync(DeviceStateChanged item)
        {
            _logger.LogInformation($" -------> Monitor: {item.DeviceTopic} -> {item.State}");

            await _homeAutomationSignalRContext.Clients.All.SendAsync(
                "DeviceStateChanged", 
                _deviceRepository.GetByDeviceTopic(item.DeviceTopic)?.Id, 
                item.State);

            _logger.LogInformation("SignalR Message sent.");
        }
    }
}
