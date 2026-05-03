namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class PresentValue
{
    public string CusipOrSymbol { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal DividendMultiple { get; set; }
    public decimal SplitMultiple { get; set; }
    public DateTime AsOfDate { get; set; }
}
