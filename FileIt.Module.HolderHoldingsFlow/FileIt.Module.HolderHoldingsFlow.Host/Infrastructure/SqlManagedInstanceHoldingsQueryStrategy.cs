using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Data;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure
{
    public class SqlManagedInstanceHoldingsQueryStrategy : IHoldingsQueryStrategy
    {
        private readonly HolderHoldingsDbContext context;
        public SqlManagedInstanceHoldingsQueryStrategy(HolderHoldingsDbContext context)
        {
            this.context = context;
        }

        public Task<IReadOnlyList<Holding>> GetCurrentHoldingsAsync(DateTime asOfDate, CancellationToken ct)
        {
            var holdingDates = context.Holdings.GroupBy(t => t.HolderId)
                .Select(g => new
                {
                    HolderId = g.Key,
                    MaxDate = g.Max(t => t.AsOfDate)
                }).ToList();
            var holdings = context.Holdings.Where(h => holdingDates.Any(hd => hd.HolderId == h.HolderId && hd.MaxDate == h.AsOfDate));
            return Task.FromResult<IReadOnlyList<Holding>>(holdings.ToList());
        }
    }
}
