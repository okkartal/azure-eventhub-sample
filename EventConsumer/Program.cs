using EventConsumer;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection("Settings").Get<Settings>();

var processor = new EventProcessor();
await processor.StartEventProcessing(settings, CancellationToken.None);