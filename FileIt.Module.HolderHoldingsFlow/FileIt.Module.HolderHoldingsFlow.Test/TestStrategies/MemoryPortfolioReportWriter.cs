using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class MemoryPortfolioReportWriter : IPortfolioReportWriter
{
    public string? LastReportName { get; private set; }
    public IReadOnlyList<PortfolioReportRow> LastRows { get; private set; }
        = Array.Empty<PortfolioReportRow>();

    public Task WriteAsync(
        string reportName,
        IReadOnlyList<PortfolioReportRow> rows,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        LastReportName = reportName;
        LastRows = rows.ToList().AsReadOnly();

        return Task.CompletedTask;
    }
}
