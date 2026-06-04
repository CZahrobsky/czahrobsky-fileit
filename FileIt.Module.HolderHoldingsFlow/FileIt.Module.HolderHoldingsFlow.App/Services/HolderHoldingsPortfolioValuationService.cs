using System;
using System.Collections.Generic;
using System.Text;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.App.Services
{
    public sealed class HolderHoldingsPortfolioValuationService
    {
        private readonly IHoldingsSnapshotImporter _snapshotImporter;
        private readonly IHolderHoldingTransactionsImporter _deltaImporter;
        private readonly IHoldingsQueryStrategy _holdingsQuery;
        private readonly IPresentValueQuoteClient _quoteClient;
        private readonly IPortfolioReportWriter _writer;

        public async Task ProcessAsync(
            Stream inputFile,
            string fileName,
            DateTime asOfDate,
            CancellationToken ct)
        {
            if (fileName.Contains("Snapshot", StringComparison.OrdinalIgnoreCase))
                await _snapshotImporter.ImportSnapshotAsync(inputFile, ct);
            else if (fileName.Contains("Update", StringComparison.OrdinalIgnoreCase))
                await _deltaImporter.ImportHolderHoldingTransactionsAsync(inputFile, ct);
            else
                throw new InvalidOperationException($"Unknown holdings file type: {fileName}");

            var holdings = await _holdingsQuery.GetCurrentHoldingsAsync(asOfDate, ct);

            var symbols = holdings
                .Select(x => x.CusipOrSymbol)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var quotes = await _quoteClient.GetQuotesAsync(symbols, asOfDate, ct);

            var rows = holdings.Select(h =>
            {
                var quote = quotes[h.CusipOrSymbol];

                var currentShares =
                    h.Quantity *
                    quote.CumulativeSplits *
                    quote.DividendMultiple;

                return new PortfolioReportRow(
                    h.HolderId,
                    h.CusipOrSymbol,
                    h.Name,
                    h.Quantity,
                    quote.CumulativeSplits,
                    quote.DividendMultiple,
                    currentShares,
                    quote.UnitPrice,
                    currentShares * quote.UnitPrice,
                    asOfDate);
            }).ToList();

            await _writer.WriteAsync($"PortfolioValue_{asOfDate:yyyyMMdd}.tsv", rows, ct);
        }
    }
}
