using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class MdbHoldingsSnapshotImporter : IHoldingsSnapshotImporter
{
    public Task ImportSnapshotAsync(Stream file, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
