using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Data;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure
{
    public class SqlManagedInstanceHoldingsSnapshotImporter : IHoldingsSnapshotImporter
    {
        private readonly HolderHoldingsDbContext context;
        public SqlManagedInstanceHoldingsSnapshotImporter(HolderHoldingsDbContext context)
        {
            this.context = context;
        }

        public Task ImportSnapshotAsync(Stream file, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
