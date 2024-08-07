using DeliveryOrderProcessor.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DeliveryOrderProcessor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCosmosDbInfrastructure(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<CosmosDbOptions>>().Value;

        services.AddLogging()
            .AddSingleton(typeof(CosmosClient), new CosmosClient(options.ConnectionString));

        return services;
    }

    public static IServiceCollection RegisterConfiguration<TOptions>(this IServiceCollection services, string sectionName)
        where TOptions : class, new()
    {
        var serviceCollection = services
            .AddOptions<TOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.Bind(sectionName, settings);
            }).Services;

        return serviceCollection;
    }
}
