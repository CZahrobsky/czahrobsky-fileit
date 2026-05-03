namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;
//     public long ValuationId { get; set; } = null!;


public class HolderHoldingValuation
{
    public string HolderId { get; set; } = null!;
    public string CusipOrSymbol { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime ValuationDate { get; set; }
}
