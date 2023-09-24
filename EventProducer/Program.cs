using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventGeneratorLibrary;
using EventGeneratorLibrary.Constants;
using Microsoft.Extensions.Configuration;
using EventProducer;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection("Settings").Get<Settings>();


await StartEventGenerating(settings);


async Task StartEventGenerating(Settings settings)
{
    await using (var producer = new EventHubProducerClient(settings.EventHubConnectionString, settings.EventHubName))
    {
        while (true)
        {
            var events = EventGenerator.GetSensorEvents(Sensors.FirstSensor, 900);

            var eventBatch = await producer.CreateBatchAsync();

            foreach (var sensorEvent in events)
            {
                var eventAddedSuccessully = eventBatch.TryAdd(new EventData(JsonSerializer.Serialize(sensorEvent)));
                if (!eventAddedSuccessully)
                {
                    if (eventBatch.Count > 0)
                    {
                        await producer.SendAsync(eventBatch);
                        Console.WriteLine(
                            $"Batch data sent for {eventBatch.Count} events out of {events.Count()} events because size limit reached");
                        eventBatch = await producer.CreateBatchAsync();
                    }
                    // event size is too big and cannot be added. Event needs to be skipped
                    //log the error
                }
            }

            if (eventBatch.Count > 0) await producer.SendAsync(eventBatch);
            eventBatch.Dispose();
        }
    }
}