using FileIt.Module.HolderHoldingsFlow.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileIt.Module.HolderHoldingsFlow.Host.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHolderHoldingsFlowServices(this IServiceCollection services)
    {
        // Register Application layer services
        services.AddScoped<HolderIngestionService>();
        services.AddScoped<HoldingIngestionService>();
        services.AddScoped<PresentValueIngestionService>();
        services.AddScoped<HoldingValuationAggregator>();

        // Register Application layer handlers
        services.AddScoped<ProcessHoldersHandler>();
        services.AddScoped<ProcessHoldingsHandler>();
        services.AddScoped<ProcessPresentValuesHandler>();
        services.AddScoped<AggregateHoldingValuationsHandler>();

        return services;
    }
}
