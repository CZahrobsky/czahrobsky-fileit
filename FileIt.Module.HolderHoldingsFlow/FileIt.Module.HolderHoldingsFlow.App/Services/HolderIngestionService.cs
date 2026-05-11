using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class HolderIngestionService : BaseHolderHoldingsFlow
{
    public async Task<List<Holder>> LoadHoldersAsync(/* parameters */)
    {
        // Load holder data from your source
        var holders = new List<Holder>();
        try
        {
            // (Database, Blob Storage, Service Bus, etc.)
            if (Source != null)
            {
                var asOfDate = DateTime.Today;
                var holderList = await BaseHolderHoldingsFlow.Source.GetHoldersAsync(asOfDate);
                holders.AddRange(holderList);
            }
        }
        catch (Exception ex)
        {
            holders.Add(new Holder
            {
                HolderId = "Error",
                CustomerName = $"Error loading holders: {ex.Message}"
            });
        }
        return holders;
    }

}
