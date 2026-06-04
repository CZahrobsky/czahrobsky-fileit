using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Data;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure
{
    public class SqlManagedInstanceHolderHoldingTransactionsImporter : IHolderHoldingTransactionsImporter
    {
        private readonly HolderHoldingsDbContext context;
        public SqlManagedInstanceHolderHoldingTransactionsImporter(HolderHoldingsDbContext context)
        {
            this.context = context;
        }

        public Task ImportHolderHoldingTransactionsAsync(Stream file, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
