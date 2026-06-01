namespace FileIt.Module.HolderHoldingsFlow.Host.Configuration;

public class HolderHoldingsFlowConfig
{
    public string BlobConnectionString { get; set; } = null!;
    public string ServiceBusConnectionString { get; set; } = null!;
    public string SqlConnectionString { get; set; } = null!;
    public string HoldersContainerName { get; set; } = "holders-input";
    public string HoldingsContainerName { get; set; } = "holdings-input";
    public string PresentValuesContainerName { get; set; } = "present-values-input";
    public string ValuationsContainerName { get; set; } = "valuations-output";
}
