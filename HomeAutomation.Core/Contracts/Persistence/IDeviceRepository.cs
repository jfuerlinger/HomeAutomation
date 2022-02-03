using HomeAutomation.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Core.Contracts.Persistence
{
    public interface IDeviceRepository
    {
        IEnumerable<Device> GetAll();
        Device? GetById(Guid id);
        Device? GetByDeviceTopic(string deviceTopic);
    }
}
