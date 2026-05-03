using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class HoldingIngestionService
{
    public async Task LoadHoldingsAsync(/* parameters */)
    {
        // Load holdings data from your source
        var holdings = new List<Holding>();
        // (Database, Blob Storage, Service Bus, etc.)
    }
}
