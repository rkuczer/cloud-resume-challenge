using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Newtonsoft.Json;
using System.Net.Http;

namespace Company.Function
{
    public class GetResumeCounter
    {
        private readonly ILogger<GetResumeCounter> _logger;

        public GetResumeCounter(ILogger<GetResumeCounter> logger)
        {
            _logger = logger;
        }

        [FunctionName("GetResumeCounter")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            [CosmosDB(
                databaseName: "KoozResume",
                containerName: "Counter",
                Connection = "KoozResumeConnectionString",
                Id = "1",
                PartitionKey = "1")] Counter counter,
            [CosmosDB(
                databaseName: "KoozResume",
                containerName: "Counter",
                Connection = "KoozResumeConnectionString",
                Id = "1",
                PartitionKey = "1")] IAsyncCollector<Counter> outputCounter)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (counter == null)
            {
                _logger.LogInformation("Counter not found.");
                var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                return notFoundResponse;
            }

            // Update the counter
            counter.Count++;

            // Add the updated counter to the output binding
            await outputCounter.AddAsync(counter);

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteAsJsonAsync(counter);

            return response;
        }
    }

}
