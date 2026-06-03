using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class MdbHoldingsQueryStrategy : IHoldingsQueryStrategy
{
    public Task<IReadOnlyList<Holding>> GetCurrentHoldingsAsync(DateTime asOfDate, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
