using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class MemoryPortfolioReportWriter : IPortfolioReportWriter
{
    public Task WriteAsync(string reportName, IReadOnlyList<PortfolioReportRow> rows, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
