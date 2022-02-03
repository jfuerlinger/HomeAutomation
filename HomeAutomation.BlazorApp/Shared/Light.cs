using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.BlazorApp.Shared
{
    public class Light
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? MqttTopic { get; set; }
    }
}
