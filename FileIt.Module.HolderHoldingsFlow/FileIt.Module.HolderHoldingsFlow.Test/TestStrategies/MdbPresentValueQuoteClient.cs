using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Test.TestStrategies;

public class MdbPresentValueQuoteClient : IPresentValueQuoteClient
{
    private readonly TestSource _source;

    public MdbPresentValueQuoteClient(TestSource source)
    {
        _source = source;
    }

    public async Task<IReadOnlyDictionary<string, PresentValue>> GetQuotesAsync(
        IEnumerable<string> symbols,
        DateTime asOfDate,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var symbolList = symbols
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var values = await _source.GetPresentValuesAsync(
            asOfDate,
            symbolList,
            SeekOrigin.Current,
            ct);

        return values
            .GroupBy(x => x.CusipOrSymbol, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g
                    .OrderByDescending(x => x.AsOfDate)
                    .First(),
                StringComparer.OrdinalIgnoreCase);
    }
}
