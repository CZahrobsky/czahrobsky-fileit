using FileIt.Infrastructure;
using FileIt.Infrastructure.Extensions;
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
        services.AddScoped<AggregateHoldingValuationService>();
        services.AddScoped<ProcessHoldersHandler>();
        services.AddScoped<ProcessHoldingsHandler>();
        services.AddScoped<ProcessPresentValuesHandler>();
        services.AddScoped<ProcessAggregateHoldingValuationsHandler>();
        // Add Application Insights - RELEASE only
        // services.AddApplicationInsightsTelemetry();
    }).Build();

host.Run();

public class HolderIngestionService
{
    public async Task LoadHoldersAsync(/* parameters */)
    {
        // Load holder data from your source
        var holders = new List<Holder>();
        // (Database, Blob Storage, Service Bus, etc.)
    }
}

public class HoldingIngestionService
{
    public async Task LoadHoldingsAsync(/* parameters */)
    {
        // Load holdings data from your source
        var holdings = new List<Holding>();
        // (Database, Blob Storage, Service Bus, etc.)
    }
}

public class PresentValueIngestionService
{
    public async Task LoadPresentValuesAsync(/* parameters */)
    {
        // Load holdings data from your source
        var pv = new List<PresentValue>();

        // (Database, Blob Storage, Service Bus, etc.)
    }
}

public class AggregateHoldingValuationService
{
    public async Task LoadAggregateHoldingValuationsAsync(/* parameters */)
    {
        // Load holdings data from your source
        var aggValuation = new List<HolderHoldingValuation>();
        // (Database, Blob Storage, Service Bus, etc.)
    }
}


public class ProcessHoldersHandler
{
    private readonly HolderIngestionService _ingestionService;

    public ProcessHoldersHandler(HolderIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadHoldersAsync();
    }
}

public class ProcessHoldingsHandler
{
    private readonly HoldingIngestionService _ingestionService;

    public ProcessHoldingsHandler(HoldingIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadHoldingsAsync();
    }
}

public class ProcessPresentValuesHandler
{
    private readonly PresentValueIngestionService _ingestionService;

    public ProcessPresentValuesHandler(PresentValueIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadPresentValuesAsync();
    }
}

public class ProcessAggregateHoldingValuationsHandler
{
    private readonly AggregateHoldingValuationService _ingestionService;

    public ProcessAggregateHoldingValuationsHandler(AggregateHoldingValuationService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadAggregateHoldingValuationsAsync();
    }
}

