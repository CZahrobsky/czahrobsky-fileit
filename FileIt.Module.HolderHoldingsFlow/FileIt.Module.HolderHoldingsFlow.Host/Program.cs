using FileIt.Infrastructure;
using FileIt.Infrastructure.Extensions;
using FileIt.Module.HolderHoldingsFlow.App.Services;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Register configuration
        services.AddOptions<HolderHoldingsFlowConfig>()
            .Configure<IConfiguration>((config, cfg) =>
            {
                cfg.GetSection("HolderHoldingsFlow").Bind(config);
            });

        // Create infrastructure config from IConfiguration 
        var infrastructureConfig = new InfrastructureConfig(context.Configuration);

        // Register shared infrastructure (Blob, Service Bus, Database)
        services.AddInfrastructure(infrastructureConfig);

        // Register Application Services
        services.AddHolderHoldingsFlowServices();
        services.AddScoped<HolderIngestionService>();
        services.AddScoped<HoldingIngestionService>();
        services.AddScoped<PresentValueIngestionService>();
        services.AddScoped<AggregateHoldingValuationsService>();
        services.AddScoped<ProcessHoldersHandler>();
        services.AddScoped<ProcessHoldingsHandler>();
        services.AddScoped<ProcessPresentValuesHandler>();
        services.AddScoped<ProcessAggregateHoldingValuationsHandler>();
        // Add Application Insights - RELEASE only
        // services.AddApplicationInsightsTelemetry();
    }).Build();

host.Run();


