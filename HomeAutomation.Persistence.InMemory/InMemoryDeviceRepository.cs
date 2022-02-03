using HomeAutomation.Core.Contracts.Persistence;
using HomeAutomation.Core.Model;

namespace HomeAutomation.Persistence.InMemory
{
    public class InMemoryDeviceRepository : IDeviceRepository
    {
        private static readonly Device[] _devices = new[]
        {
            new Device { Id= Guid.NewGuid(), Name="Büro", MqttTopic="wohnung/buero/deckenlicht"}
        };

        public IEnumerable<Device> GetAll() => _devices;

        public Device? GetByDeviceTopic(string deviceTopic) => _devices.Single(device => device.MqttTopic == deviceTopic);

        public Device? GetById(Guid id) => _devices.Single(device => device.Id == id);
    }
}