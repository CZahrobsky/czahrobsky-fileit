using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class AggregateHoldingValuationsService
{
    public async Task LoadAggregateHoldingValuationsAsync(/* parameters */)
    {
        // Load holdings data from your source
        var aggValuation = new List<HolderHoldingValuation>();
        // (Database, Blob Storage, Service Bus, etc.)
    }
}
