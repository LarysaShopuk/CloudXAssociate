using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Models;
using System;

namespace OrderItemsReserver
{
    public static class OrderItemsReserverFunction
    {
        [FunctionName("OrderItemsReserverFunction")]
        public static async Task Run(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")]
            string orderItem,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            [Blob("reservations/{rand-guid}.json", FileAccess.Write)] BlobClient outputBlobClient,
            ILogger log)
        {
            ProductReservation productReservation;
            // deserialize for validation purpose
            productReservation = JsonConvert.DeserializeObject<ProductReservation>(orderItem);
            var json = JsonConvert.SerializeObject(orderItem);
            if (productReservation == null)
            {
                log.LogError("Wrong reservation message format passed");

                return;
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
                throw;
            }

            log.LogInformation($"Made reservation for '{productReservation.Id}' product successfully");
        }
    }
}
