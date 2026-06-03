using System;
using System.Collections.Generic;
using System.Text;

namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities
{
    public class HolderHoldingTransaction
    {
        public long Id { get; set; }
        public string TransactionType { get; set; } = null!;
        public string HolderId { get; set; } = null!;
        public string CusipOrSymbol { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Quantity { get; set; }
        public DateTime AsOfDate { get; set; }

    }
}
