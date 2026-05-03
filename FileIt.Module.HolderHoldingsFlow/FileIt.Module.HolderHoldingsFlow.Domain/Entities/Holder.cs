namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

public class Holder
{
    public string HolderId { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Zip { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
