using System;
using System.Collections.Generic;
using System.Text;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Strategies
{
    public interface IHoldingsSnapshotImporter
    {
        Task ImportSnapshotAsync(Stream file, CancellationToken ct);
    }

    public interface IHolderHoldingTransactionsImporter
    {
        Task ImportHolderHoldingTransactionsAsync(Stream file, CancellationToken ct);
    }

    public interface IHoldingsQueryStrategy
    {
        Task<IReadOnlyList<Holding>> GetCurrentHoldingsAsync(DateTime asOfDate, CancellationToken ct);
    }

    public interface IPresentValueQuoteClient
    {
        Task<IReadOnlyDictionary<string, PresentValue>> GetQuotesAsync(
            IEnumerable<string> symbols,
            DateTime asOfDate,
            CancellationToken ct);
    }

    public interface IPortfolioReportWriter
    {
        Task WriteAsync(string reportName, IReadOnlyList<PortfolioReportRow> rows, CancellationToken ct);
    }
}
