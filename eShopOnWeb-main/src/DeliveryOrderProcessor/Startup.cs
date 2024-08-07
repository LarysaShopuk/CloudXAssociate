using DeliveryOrderProcessor;
using DeliveryOrderProcessor.Extensions;
using DeliveryOrderProcessor.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DeliveryOrderProcessor;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .RegisterConfiguration<CosmosDbOptions>(nameof(CosmosDbOptions));
    }
}
