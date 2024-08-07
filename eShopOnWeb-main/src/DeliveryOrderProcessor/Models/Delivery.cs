using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeliveryOrderProcessor.Models;
public class Delivery
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    public string CustomerName { get; set; }

    public string ShippingAddress { get; set; }

    public IList<OrderItem> Items { get; set; } = new List<OrderItem>();

    public decimal Price { get; set; }
}
