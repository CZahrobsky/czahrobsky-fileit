using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class MdbHolderHoldingTransactionsImporter : IHolderHoldingTransactionsImporter
{
    public Task ImportDeltaAsync(Stream file, CancellationToken ct)
    {
        throw new NotImplementedException(); 
    }
}
