using HomeAutomation.BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace HomeAutomation.BlazorApp.Client.Pages
{
    public partial class Dashboard : IAsyncDisposable
    {
        [Inject]
        HttpClient? Http { get; set; }

        [Inject]
        NavigationManager? NavigationManager { get; set; }

        [Inject]
        IJSRuntime? JS { get; set; }

        private readonly List<string> _messages = new();

        private HubConnection? _hubConnection;
        private Light[]? _lights;

        protected override async Task OnInitializedAsync()
        {
            ArgumentNullException.ThrowIfNull(Http, nameof(Http));
            ArgumentNullException.ThrowIfNull(NavigationManager, nameof(NavigationManager));
            ArgumentNullException.ThrowIfNull(JS, nameof(JS));

            _lights = await Http.GetFromJsonAsync<Light[]>("Devices");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/home-automation"))
                .Build();

            _hubConnection.On<string, string>("DeviceStateChanged", async (id, state) =>
            {
                var encodedMsg = $"{id}: {state}";
                _messages.Add(encodedMsg);
                StateHasChanged();

                await JS.InvokeVoidAsync("changeDeviceByState", id, state);
            });

            await _hubConnection.StartAsync();
        }

        async Task SendMqttCmdAsync(Light light, string command)
        {
            ArgumentNullException.ThrowIfNull(Http, nameof(Http));

            await Http.PostAsync($"Devices/{light.Id}/{command}", null);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }

        private string GetCssClassByState(string? state) => state?.ToLower() == "on" ? "light-enabled" : "light-disabled";

    }
}
