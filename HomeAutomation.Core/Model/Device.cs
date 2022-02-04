namespace HomeAutomation.Core.Model
{
    public class Device
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? DeviceTopic { get; set; }
        public string? State { get; set; }
        public DateTime? LastStateCheckedAt { get; set; }
    }
}
