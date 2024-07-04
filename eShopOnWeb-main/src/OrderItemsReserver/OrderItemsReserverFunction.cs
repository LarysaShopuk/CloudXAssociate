using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Models;
using System;

namespace OrderItemsReserver
{
    public static class OrderItemsReserverFunction
    {
        [FunctionName("OrderItemsReserverFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("reservations/{rand-guid}.json", FileAccess.Write)] BlobClient outputBlobClient,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ProductReservation productReservation;
            try
            {
                // deserialize for validation purpose
                productReservation = JsonConvert.DeserializeObject<ProductReservation>(requestBody);
                var json = JsonConvert.SerializeObject(requestBody);
                if (productReservation == null)
                {
                    return new BadRequestObjectResult("Got NULL product reservation object");
                }
                log.LogInformation($"Start to make reservation for '{productReservation.Id}' product");
                try
                {
                    await outputBlobClient.UploadAsync(BinaryData.FromString(json));
                }
                catch (Exception e)
                {
                    var message = $"Failed to upload file with reservation details.";
                    log.LogError(e, message);
                }

                log.LogInformation($"Made reservation for '{productReservation.Id}' product successfully");
            }
            catch
            {
                return new BadRequestObjectResult("Got unexpected product reservation object");
            }

            return new OkObjectResult($"Reservation for '{productReservation.Id}' product was created successfully");
        }
    }
}
