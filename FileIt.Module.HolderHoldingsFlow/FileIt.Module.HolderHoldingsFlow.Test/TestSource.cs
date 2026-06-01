using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Domain.Interfaces;

namespace FileIt.Module.HolderHoldingsFlow.Test
{
    internal class TestSource : IHolderHoldingsFlowSource
    {
        public Task<IReadOnlyList<HolderHoldingValuation>> CalculateValuations(DateTime asOfDate, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Holder>> GetHoldersAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            return ReadJsonAsync<Holder>("Holders", ct).ContinueWith(t => (IReadOnlyList<Holder>)t.Result.AsReadOnly());
        }


        public Task<IReadOnlyList<Holding>> GetHoldingsAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            return ReadJsonAsync<Holding>("Holdings", ct).ContinueWith(t => (IReadOnlyList<Holding>)t.Result.AsReadOnly());
        }

        public Task<IReadOnlyList<PresentValue>> GetPresentValuesAsync(DateTime asOfDate, IList<string> CusipOrSymbolList, CancellationToken ct = default)
        {
            var filteredListTask = ReadJsonAsync<PresentValue>("PresentValues", ct).ContinueWith(t =>
            {
                var list = t.Result;
                if (CusipOrSymbolList == null || CusipOrSymbolList.Count == 0) return list;
                return list.Where(pv => CusipOrSymbolList.Contains(pv.CusipOrSymbol, StringComparer.OrdinalIgnoreCase)).ToList();
            });
            return filteredListTask.ContinueWith(t => (IReadOnlyList<PresentValue>)t.Result.AsReadOnly());
        }

        private async Task<List<T>> ReadJsonAsync<T>(string dataSource, CancellationToken ct)
        {
            var list = new List<T>();
            var asm = typeof(TestSource).Assembly;
            var resName = asm.GetManifestResourceNames().FirstOrDefault(n => n.Contains(dataSource, StringComparison.OrdinalIgnoreCase));
            if (resName == null) return list;

            try
            {
                using (var testData = asm.GetManifestResourceStream(resName))
                {
                    if (testData == null) return list;
                    string? json = null;
                    TextReader reader = new StreamReader(testData, Encoding.UTF8);
                    if (resName.ToLower().EndsWith(".zip"))
                    {
                        // If zip file, stream to a memory reader and set reader to a TextReader reading the first file in the zip
                        using var zip = new System.IO.Compression.ZipArchive(testData, System.IO.Compression.ZipArchiveMode.Read);
                        var entry = zip.Entries.FirstOrDefault();
                        if (entry == null) return list;
                        var zipReader = new StreamReader(entry.Open(), Encoding.UTF8);
                        json = await zipReader.ReadToEndAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        json = await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                    if (string.IsNullOrWhiteSpace(json)) return list;
                    var items = JsonSerializer.Deserialize<List<T>>(json);
                    if (items != null) list = items;
                }
            }
            catch { /* ignore deserialization errors for test helper */ }
            return list;
        }


    }
}
