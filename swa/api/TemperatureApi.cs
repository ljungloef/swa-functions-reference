using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;

namespace Functions
{
    public static class TemperatureApi
    {
        [FunctionName("TemperatureApi")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
                        [Table("Temperatures", "1", Connection = "TemperaturesDatabaseConnection")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = table.CreateQuery<TableData>();
            query.TakeCount = 100;

            var temperatures = (await table.ExecuteQuerySegmentedAsync(query, null)).ToList();

            if (temperatures.Any())
            {
                var result = new ApiResponse(
                    temperatures.First().Temperature, 
                    temperatures.Average(temp => temp.Temperature)
                );

                return new OkObjectResult(result);
            }

            return new OkResult();
        }
    }

    public record ApiResponse(double CurrentTemperature, double AverageTemperature);

    public class TableData : TableEntity
    {
        public double Temperature { get; set; }
    }
}
