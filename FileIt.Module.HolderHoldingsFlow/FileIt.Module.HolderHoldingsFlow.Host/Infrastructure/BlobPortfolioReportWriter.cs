using System.Text;
using FileIt.Domain.Interfaces;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure;

public class BlobPortfolioReportWriter : IPortfolioReportWriter
{
    private readonly IHandleFiles _blobTool;
    private readonly HolderHoldingsFlowConfig _config;
    private readonly ILogger<BlobPortfolioReportWriter> _logger;

    public BlobPortfolioReportWriter(
        IHandleFiles blobTool,
        IOptions<HolderHoldingsFlowConfig> options,
        ILogger<BlobPortfolioReportWriter> logger)
    {
        _blobTool = blobTool;
        _config = options.Value;
        _logger = logger;
    }

    public async Task WriteAsync(
        string reportName,
        IReadOnlyList<PortfolioReportRow> rows,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var tsv = BuildTsv(rows);
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(tsv));

        await _blobTool.UploadAsync(
            stream,
            reportName,
            _config.ValuationsContainerName,
            ct);

        _logger.LogInformation(
            "Uploaded portfolio valuation report {ReportName} with {RowCount} rows to {Container}",
            reportName,
            rows.Count,
            _config.ValuationsContainerName);
    }

    private static string BuildTsv(IEnumerable<PortfolioReportRow> rows)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            "HolderId\tCusipOrSymbol\tName\tQuantity\tCumulativeSplits\tDividendMultiple\tCurrentShares\tUnitPrice\tMarketValue\tAsOfDate");

        foreach (var r in rows)
        {
            sb.AppendLine(string.Join('\t',
                r.HolderId,
                r.CusipOrSymbol,
                r.Name,
                r.Quantity,
                r.CumulativeSplits,
                r.DividendMultiple,
                r.CurrentShares,
                r.UnitPrice,
                r.MarketValue,
                r.AsOfDate.ToString("yyyy-MM-dd")));
        }

        return sb.ToString();
    }
}
