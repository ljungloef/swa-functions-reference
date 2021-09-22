using System;
using AzNan.DeviceSimulator;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzNan.MessageReader
{
    public static class MessageReader
    {
        [Function("MessageReader")]
        [TableOutput("Temperatures", Connection = "AzureWebJobsStorage")]
        public static TableData Run([QueueTrigger("temperatures", Connection = "AzureWebJobsStorage")] NewTemperatureReading temperature,
            FunctionContext context)
        {
            var logger = context.GetLogger("MessageReader");
            logger.LogInformation($"C# Queue trigger function processed: {temperature}");

            return new TableData 
            {
                PartitionKey = temperature.DeviceId,
                RowKey = $"{(DateTimeOffset.MaxValue.Ticks-temperature.Timestamp.Ticks):d10}-{Guid.NewGuid():N}",
                Temperature = temperature.Temperature
            };
        }
    }

    public class TableData
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public double Temperature { get; set; }
    }
}
