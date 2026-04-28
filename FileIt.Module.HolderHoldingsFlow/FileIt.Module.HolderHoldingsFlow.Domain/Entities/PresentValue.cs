namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class PresentValue
{
    public string PresentValueId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public string Cusip { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
