using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class TemperatureProcessor
    {
        private readonly ILogger _logger;

        public TemperatureProcessor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TemperatureProcessor>();
        }

        [Function("TemperatureProcessor")]
        [TableOutput("Temperatures", Connection = "AzureWebJobsStorage")]
        public TableData Run([QueueTrigger("temperature-reports", Connection = "AzureWebJobsStorage")] NewTemperatureReading temperature)
        {
            if (temperature == null)
            {
                _logger.LogError($"Temperature null");
                throw new InvalidOperationException();
            }

            _logger.LogInformation($"Processing temperature reading from {temperature.DeviceId}");

            return new TableData(
                temperature.DeviceId, 
                $"{(DateTimeOffset.MaxValue.Ticks-temperature.Timestamp.Ticks):d10}-{Guid.NewGuid():N}", 
                temperature.Temperature
            );
        }
    }

    public record TableData(string PartitionKey, string RowKey, double Temperature);
}
