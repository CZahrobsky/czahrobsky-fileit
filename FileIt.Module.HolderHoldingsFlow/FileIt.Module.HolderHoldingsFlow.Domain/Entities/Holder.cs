namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class Holder
{
    public string HolderId { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
