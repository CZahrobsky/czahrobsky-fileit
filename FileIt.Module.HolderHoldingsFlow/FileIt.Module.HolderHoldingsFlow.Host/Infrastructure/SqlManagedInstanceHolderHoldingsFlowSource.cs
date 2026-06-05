using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Domain.Interfaces;
using FileIt.Module.HolderHoldingsFlow.Host.Data;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure;

    public class SqlManagedInstanceHolderHoldingsFlowSource : IHolderHoldingsFlowSource
    {
        private readonly HolderHoldingsDbContext context;
        public SqlManagedInstanceHolderHoldingsFlowSource(HolderHoldingsDbContext context)
        {
            this.context = context;
        }

        public Task<IReadOnlyList<Holder>> GetHoldersAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            var task = Task.Run(() =>
            {
                var holderDates = context.Holders.GroupBy(t => t.HolderId)
                    .Select(g => new
                    {
                        HolderId = g.Key,
                        CloseDate = g.Max(t => t.AccountClosed)
                    }).ToList();
                var holders = context.Holders.Where(h => holderDates.Any(hd => hd.HolderId == h.HolderId && (hd.CloseDate == null || hd.CloseDate >= asOfDate)));
                return holders.ToList();
            });

            return task.ContinueWith(t =>
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetHoldersAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<Holder>)new List<Holder>().AsReadOnly();
                }
                return (IReadOnlyList<Holder>)t.Result.AsReadOnly();
            }, ct);
        }

        public Task<IReadOnlyList<Holding>> GetHoldingsAsync(DateTime asOfDate, CancellationToken ct)
        {
            var task = Task.Run(() =>
            {
                var holdingDates = context.Holdings.GroupBy(t => t.HolderId)
                    .Select(g => new
                    {
                        HolderId = g.Key,
                        MaxDate = g.Max(t => t.AsOfDate)
                    }).ToList();
                var holdings = context.Holdings.Where(h => holdingDates.Any(hd => hd.HolderId == h.HolderId && hd.MaxDate == h.AsOfDate));
                return holdings.ToList();
            });

            return task.ContinueWith(t =>
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetHoldingsAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<Holding>)new List<Holding>().AsReadOnly();
                }
                return (IReadOnlyList<Holding>)t.Result.AsReadOnly();
            }, ct);
        }
        
        public Task<IReadOnlyList<PresentValue>> GetPresentValuesAsync(DateTime asOfDate, IList<string> CusipOrSymbolList, SeekOrigin scope = SeekOrigin.Current, CancellationToken ct = default)
        {
            var task = Task.Run(() =>
            {
                var pv = context.PresentValues.Where(h => CusipOrSymbolList.Any(c => h.CusipOrSymbol == c));

                if (scope == SeekOrigin.Begin)
                {
                    return pv.Where(p => p.AsOfDate <= asOfDate).ToList();
                }
                if (scope == SeekOrigin.End)
                {
                    return pv.Where(p => p.AsOfDate >= asOfDate).ToList();
                }
                else
                {
                    return pv.Where(p => p.AsOfDate == asOfDate).ToList();
                }
            });

            return task.ContinueWith(t =>
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetPresentValuesAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<PresentValue>)new List<PresentValue>().AsReadOnly();
                }
                return (IReadOnlyList<PresentValue>)t.Result.AsReadOnly();
            }, ct);
        }

        public Task<IReadOnlyList<HolderHoldingTransaction>> GetTransactionsAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            var task = Task.Run(() =>
            {
                var tx = context.HolderHoldingTransactions.Where(h => h.AsOfDate == asOfDate);
                return tx.ToList();
            });

            return task.ContinueWith(t =>
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetTransactionsAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<HolderHoldingTransaction>)new List<HolderHoldingTransaction>().AsReadOnly();
                }
                return (IReadOnlyList<HolderHoldingTransaction>)t.Result.AsReadOnly();
            }, ct);
        }
    }
