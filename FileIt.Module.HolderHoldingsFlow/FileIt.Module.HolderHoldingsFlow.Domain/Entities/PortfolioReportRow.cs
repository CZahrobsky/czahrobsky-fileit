using System;
using System.Collections.Generic;
using System.Text;

namespace FileIt.Module.HolderHoldingsFlow.Domain.Entities;

    public class PortfolioReportRow
    {

        public PortfolioReportRow(string holderId, string cusipOrSymbol, string name, decimal quantity, decimal cumulativeSplits, decimal dividendMultiple, decimal currentShares, decimal unitPrice, decimal value, DateTime asOfDate)
        {
            this.HolderId = holderId;
            this.CusipOrSymbol = cusipOrSymbol;
            this.Name = name;
            this.Quantity = quantity;
            this.CumulativeSplits = cumulativeSplits;
            this.DividendMultiple = dividendMultiple;
            this.CurrentShares = currentShares;
            this.UnitPrice = unitPrice;
            this.MarketValue = value;
            this.AsOfDate = asOfDate;
        }

        public string HolderId { get; set; }
        public string CusipOrSymbol { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal CumulativeSplits { get; set; }
        public decimal DividendMultiple { get; set; }
        public decimal CurrentShares { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal MarketValue { get; set; }
        public DateTime AsOfDate { get; set; }
    }
