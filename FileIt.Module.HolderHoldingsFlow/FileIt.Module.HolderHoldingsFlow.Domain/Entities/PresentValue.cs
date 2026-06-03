namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class PresentValue
{
    public long Id { get; set; }
    public string CusipOrSymbol { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal DividendMultiple { get; set; }
    public decimal SplitMultiple { get; set; }
    public decimal CumulativeSplits { get; set; }
    public decimal? RiskScalar { get; set; }
    public DateTime AsOfDate { get; set; }
}
