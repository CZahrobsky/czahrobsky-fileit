namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class HolderHoldingValuation
{
    public string ValuationId { get; set; } = null!;
    public string HolderId { get; set; } = null!;
    public string HoldingId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime ValuationDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
