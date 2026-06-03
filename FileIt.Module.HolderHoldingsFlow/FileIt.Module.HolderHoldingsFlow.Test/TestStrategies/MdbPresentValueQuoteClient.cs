using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

/// <summary>
/// Market Data API Client
/// </summary>
public class MdbPresentValueQuoteClient : IPresentValueQuoteClient
{
    public Task<IReadOnlyDictionary<string, PresentValue>> GetQuotesAsync(
            IEnumerable<string> symbols,
            DateTime asOfDate,
            CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
