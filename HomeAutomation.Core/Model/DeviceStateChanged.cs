namespace HomeAutomation.Core.Model
{
    public class DeviceStateChanged
    {
        public string DeviceTopic { get; init; }
        public string State { get; init; }

        public DeviceStateChanged(string deviceTopic, string state)
        {
            DeviceTopic = deviceTopic;
            State = state;
        }
    }
}
