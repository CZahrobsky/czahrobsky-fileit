using System;
using System.Collections.Generic;
using System.Text;
using FileIt.Module.HolderHoldingsFlow.Domain.Interfaces;

namespace FileIt.Module.HolderHoldingsFlow.App.Services;

    public class BaseHolderHoldingsFlow
    {
        public static IHolderHoldingsFlowSource Source { get; set; } = null!;
    }
