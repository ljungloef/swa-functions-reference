using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzNan.DeviceSimulator
{
    public class NewTemperatureReading
    {
        public string DeviceId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public double Temperature { get; set; }
    }

    public static class DeviceSimulator
    {
        [Function("DeviceSimulator")]
        [QueueOutput("temperatures", Connection = "AzureWebJobsStorage")]
        public static NewTemperatureReading Run([TimerTrigger("*/30 * * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("DeviceSimulator");
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var rng = new Random();

            var reading = new NewTemperatureReading {
                DeviceId = "1",
                Timestamp = DateTimeOffset.UtcNow,
                Temperature = rng.NextDouble() * 100
            };

            return reading;
        }
    }

    public class MyInfo
    {

        public bool IsPastDue { get; set; }
    }
}
