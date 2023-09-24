using EventGeneratorLibrary.Models;

namespace EventGeneratorLibrary;

public static class EventGenerator
{
    public static IEnumerable<SensorEvent> GetSensorEvents(string sensorType, int events = 50)
    {
        var generatedEvents = 0;

        while (generatedEvents < events)
        {
            generatedEvents += 1;

            yield return new SensorEvent(sensorType)
            {
                Priority = 1,
                AccountId = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                SensorMessage = new string('*', 10000)
            };
        }
    }
}