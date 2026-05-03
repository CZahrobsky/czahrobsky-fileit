namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class Holding
{
    public string HolderId { get; set; } = null!;
    public string CusipOrSymbol { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime AsOfDate { get; set; }
}
