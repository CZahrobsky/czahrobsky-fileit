using System;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure
{
    public class BlobPortfolioReportWriter : IPortfolioReportWriter
    {
        public Task WriteAsync(string reportName, IReadOnlyList<PortfolioReportRow> rows, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
