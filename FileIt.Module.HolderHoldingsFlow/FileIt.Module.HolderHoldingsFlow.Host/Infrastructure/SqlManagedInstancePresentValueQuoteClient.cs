using System;
using FileIt.Infrastructure.Data;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure
{
    public class SqlManagedInstancePresentValueQuoteClient : IPresentValueQuoteClient
    {
        private readonly HolderHoldingsDbContext context;
        public SqlManagedInstancePresentValueQuoteClient(HolderHoldingsDbContext context)
        {
            this.context = context;
        }

        public Task<IReadOnlyDictionary<string, PresentValue>> GetQuotesAsync(
                IEnumerable<string> symbols,
                DateTime asOfDate,
                CancellationToken ct)
        {
            var quotes = context.PresentValues
                .Where(pv => symbols.Contains(pv.CusipOrSymbol) && pv.AsOfDate == asOfDate)
                .ToDictionary(pv => pv.CusipOrSymbol, pv => new PresentValue
                {
                    Id = pv.Id,
                    CusipOrSymbol = pv.CusipOrSymbol,
                    UnitPrice = pv.UnitPrice,
                    DividendMultiple = pv.DividendMultiple,
                    SplitMultiple = pv.SplitMultiple,
                    CumulativeSplits = pv.CumulativeSplits,
                    RiskScalar = pv.RiskScalar,
                    AsOfDate = pv.AsOfDate
                });

            return Task.FromResult((IReadOnlyDictionary<string, PresentValue>)quotes);
        }
    }
}
