
using HomeAutomation.Core.Contracts.Services;
using HomeAutomation.Services;
using HomeAutomation.TestDriver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IMqttService mqttService =
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IMqttService, MqttService>();
            }).ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Warning);
            })
        .Build()

            .Services
                .GetRequiredService<IMqttService>();

mqttService.Subscribe(new DeviceStateObserver(new string[] { 
            "wohnung/buero/deckenlicht",
            "wohnung/kinderzimmer/deckenlicht" }));
await mqttService.InitAsync(CancellationToken.None);


while (true)
{
    Console.WriteLine("Press any key to toggle the Light ...");
    Console.ReadKey();
    await mqttService.PublishAsync("cmnd/wohnung/buero/deckenlicht/POWER", "TOGGLE", CancellationToken.None);
    Console.WriteLine("  -> Toggled the light");
}
