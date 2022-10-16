using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class DeviceSimulator
    {
        private readonly ILogger _logger;

        public DeviceSimulator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DeviceSimulator>();
        }

        [Function("DeviceSimulator")]
        [QueueOutput("temperature-reports", Connection = "AzureWebJobsStorage")]
        public NewTemperatureReading Run([TimerTrigger("*/30 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var rng = new Random();
            var reading = new NewTemperatureReading("1", DateTimeOffset.UtcNow, rng.NextDouble() * 100);
            return reading;
        }
    }

    public record NewTemperatureReading(string DeviceId, DateTimeOffset Timestamp, double Temperature);

    public class MyInfo
    {
        public bool IsPastDue { get; set; }
    }
}
