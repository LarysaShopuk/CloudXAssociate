using System;
using System.IO;
using System.Threading.Tasks;
using DeliveryOrderProcessor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DeliveryOrderProcessor
{
    public class DeliveryOrderProcessor
    {
        [FunctionName("DeliveryOrderProcessor")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            [CosmosDB(databaseName: "eshopDb",
                "deliverables",
                Connection = "cosmosDBConnectionString",
                CreateIfNotExists = true,
                PartitionKey = "/CustomerName")]
            IAsyncCollector<Delivery> collector,
            ILogger log)
        {
            log.LogInformation("Start processing new order delivery");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Delivery delivery = null;
            try
            {
                delivery = JsonConvert.DeserializeObject<Delivery>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Delivery couldn't be parsed from the the request payload");
            }

            if (delivery == null)
            {
                throw new Exception("Invalid payload was passed");
            }

            try
            {
                await collector.AddAsync(delivery);
                log.LogInformation($"'{delivery.Id}'order was sent to Delivery service");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Failed to send '{delivery.Id}'order to Delivery service");
            }

            return new CreatedResult(string.Empty, delivery);
        }
    }
}
