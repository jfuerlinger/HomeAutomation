using HomeAutomation.Core.Model;

namespace HomeAutomation.TestDriver
{
    internal class DeviceStateObserver : IObserver<DeviceStateChanged>
    {
        private readonly List<string> _deviceTopicsToObserve = new();

        public DeviceStateObserver(string deviceTopicToObserve)
        {
            _deviceTopicsToObserve.Add(deviceTopicToObserve);
        }

        public DeviceStateObserver(IEnumerable<string> deviceTopicsToObserve)
        {
            _deviceTopicsToObserve.AddRange(deviceTopicsToObserve);
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
                Console.WriteLine($"DeviceTopic='{value.DeviceTopic}' State='{value.State}'");
                Console.Beep();
            }
        }
    }
}