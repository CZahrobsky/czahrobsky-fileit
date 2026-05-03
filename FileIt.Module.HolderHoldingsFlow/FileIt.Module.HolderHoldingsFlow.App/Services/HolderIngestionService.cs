using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class HolderIngestionService
{
    public async Task LoadHoldersAsync(/* parameters */)
    {
        // Load holder data from your source
        var holders = new List<Holder>();
        // (Database, Blob Storage, Service Bus, etc.)
    }

}
