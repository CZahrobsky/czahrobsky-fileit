using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class HoldingIngestionService : BaseHolderHoldingsFlow
{
    public async Task<List<Holding>> LoadHoldingsAsync(/* parameters */)
    {
        // Load holding data from your source
        var holdings = new List<Holding>();
        try
        {
            // (Database, Blob Storage, Service Bus, etc.)
            if (Source != null)
            {
                var asOfDate = DateTime.Today;
                var holdingList = await Source.GetHoldingsAsync(asOfDate);
                holdings.AddRange(holdingList);
            }
        }
        catch (Exception ex)
        {
            holdings.Add(new Holding
            {
                CusipOrSymbol = "Error",
                Name = $"Error loading holdings: {ex.Message}"
            });
        }
        return holdings;
    }
}
