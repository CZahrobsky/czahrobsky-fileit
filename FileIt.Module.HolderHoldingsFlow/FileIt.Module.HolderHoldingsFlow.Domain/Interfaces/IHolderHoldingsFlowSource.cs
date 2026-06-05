using System;
using System.Collections.Generic;
using System.IO;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;

namespace FileIt.Module.HolderHoldingsFlow.Domain.Interfaces;

    public interface IHolderHoldingsFlowSource
    {
        Task<IReadOnlyList<Holder>> GetHoldersAsync(DateTime asOfDate, CancellationToken ct = default);
        Task<IReadOnlyList<Holding>> GetHoldingsAsync(DateTime asOfDate, CancellationToken ct = default);
        Task<IReadOnlyList<PresentValue>> GetPresentValuesAsync(DateTime asOfDate, IList<string> CusipOrSymbolList, SeekOrigin scope, CancellationToken ct = default);
        Task<IReadOnlyList<HolderHoldingTransaction>> GetTransactionsAsync(DateTime asOfDate, CancellationToken ct = default);
    }
