namespace EventConsumerClient;

public class Settings
{
    public string StorageClient { get; set; }

    public string EventHubConnectionString { get; set; }

    public string BlobContainerName { get; set; }

    public string EventHubName { get; set; }

    public string ConsumerGroup { get; set; }
}