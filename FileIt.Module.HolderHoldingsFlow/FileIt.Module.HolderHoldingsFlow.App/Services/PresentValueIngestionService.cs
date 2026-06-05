using System.IO;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class PresentValueIngestionService : BaseHolderHoldingsFlow
{
    public async Task<List<PresentValue>> LoadPresentValuesAsync(List<string> cusipOrSymbolList, SeekOrigin scope = SeekOrigin.Current)
    {
        // Load present values from your source / API
        var presentValues = new List<PresentValue>();
        try
        {
            // (Database, Blob Storage, Service Bus, etc.)
            if (Source != null)
            {
                var asOfDate = DateTime.Today;
                var pvList = await Source.GetPresentValuesAsync(asOfDate, cusipOrSymbolList, scope);
                presentValues.AddRange(pvList);
            }
        }
        catch (Exception ex)
        {
            presentValues.Add(new PresentValue
            {
                CusipOrSymbol = $"Error loading present values: {ex.Message}"
            });
        }
        return presentValues;
    }

}
