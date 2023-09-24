using System.Collections.Concurrent;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace EventConsumer;

public class EventProcessor
{
    private readonly ConcurrentDictionary<string, int> _partitionEventCount = new();

    public async Task StartEventProcessing(Settings settings, CancellationToken cToken)
    {
        var storageClient = new BlobContainerClient(settings.StorageClient, settings.BlobContainerName);

        var processor = new EventProcessorClient(storageClient, settings.ConsumerGroup,
            settings.EventHubConnectionString, settings.EventHubName);

        processor.ProcessEventAsync += HandleEventProcessing;
        processor.ProcessErrorAsync += HandleEventError;

        try
        {
            await processor.StartProcessingAsync(cToken);
            await Task.Delay(Timeout.Infinite, cToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error : {ex.Message}");
        }
        finally
        {
            await processor.StopProcessingAsync();
            processor.ProcessEventAsync -= HandleEventProcessing;
            processor.ProcessErrorAsync -= HandleEventError;
        }
    }

    private async Task HandleEventProcessing(ProcessEventArgs args)
    {
        try
        {
            if (args.CancellationToken.IsCancellationRequested) return;

            var partition = args.Partition.PartitionId;
            var eventBody = args.Data.EventBody.ToArray();

            var eventsSinceLastCheckpoint = _partitionEventCount.AddOrUpdate(
                partition,
                1,
                (_, currentCount) => currentCount + 1);
            Console.WriteLine($"Events since last checkpoint: {eventsSinceLastCheckpoint}");
            await args.UpdateCheckpointAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error : {ex.Message}");
        }
    }

    private Task HandleEventError(ProcessErrorEventArgs args)
    {
        Console.WriteLine("Error in the EventProcessorClient");
        Console.WriteLine($"\tOperation: {args.Operation}\n\tException: {args.Exception}\n ");
        return Task.CompletedTask;
    }
}