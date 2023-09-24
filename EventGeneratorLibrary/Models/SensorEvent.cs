namespace EventGeneratorLibrary.Models;

public class SensorEvent
{
    public SensorEvent(string sensorType)
    {
        SensorType = sensorType;
    }

    public string SensorType { get; }
    public DateTime TimeStamp { get; set; }
    public Guid AccountId { get; set; }
    public int Priority { get; set; }
    public string SensorMessage { get; set; }
}