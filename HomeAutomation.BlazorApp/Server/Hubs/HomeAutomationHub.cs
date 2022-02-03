using Microsoft.AspNetCore.SignalR;

namespace HomeAutomation.BlazorApp.Server.Hubs
{
    public class HomeAutomationHub : Hub
    {
        public async Task SendDeviceStateChangedMessage(string id, string state)
        {
            await Clients.All.SendAsync("DeviceStateChanged", id, state);
        }
    }
}
