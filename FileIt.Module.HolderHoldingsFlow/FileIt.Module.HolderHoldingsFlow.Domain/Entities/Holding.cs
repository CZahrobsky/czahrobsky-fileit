namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class Holding
{
    public string HoldingId { get; set; } = null!;
    public string HolderId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public string Cusip { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime ValuationDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
