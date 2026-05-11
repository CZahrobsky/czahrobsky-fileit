using FileIt.Infrastructure;
using FileIt.Infrastructure.Extensions;
using FileIt.Infrastructure.Logging;
using FileIt.Infrastructure.Middleware;
using FileIt.Module.HolderHoldingsFlow.App.Services;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Test;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class HolderHoldingsFlowTest
{

    [TestMethod]
    public void TestHolders()
    {
        HolderIngestionService holderIngest = new HolderIngestionService();
        HolderIngestionService.Source = new TestSource();
        var holders = holderIngest.LoadHoldersAsync().GetAwaiter().GetResult();

    }

}
