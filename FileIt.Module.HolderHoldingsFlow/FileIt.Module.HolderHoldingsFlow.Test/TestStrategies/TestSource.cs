using System.Data;
using System.Data.OleDb;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FileIt.Infrastructure.Data;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Domain.Interfaces;

namespace FileIt.Module.HolderHoldingsFlow.Test.TestStrategies;

    public class TestSource : IHolderHoldingsFlowSource
    {        
        static string mdbFile = "";
        internal static char qt = '\"';
        public string ConnectionString { get; set; } = "";

        public TestSource()
        {
            // Unzip the embedded MDB file resource to a temporary location for testing
            mdbFile = UnzipResourceToTemp("FileIt.zip").FirstOrDefault() ?? "N/A";
            Assert.IsTrue(File.Exists(mdbFile), "Failed to extract MDB file for testing.");
            ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={mdbFile};";
        }

        public Task<IReadOnlyList<HolderHoldingValuation>> CalculateValuations(DateTime asOfDate, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Holder>> GetHoldersAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                var db = GetRows("SELECT * FROM HHF_Holders WHERE AsOfDate = #" + asOfDate.ToString("yyyyMMdd") + "#");
                var list = new List<Holder>();
                foreach (DataRow row in db.Rows)
                {
                    // Check for cancellation - exit gracefully without throwing
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine($"GetHoldersAsync: Cancellation requested. Returning {list.Count} values retrieved so far.");
                        break;
                    }
                    list.Add(row.Hydrate<Holder>());
                }
                return list;
            }, ct).ContinueWith(t => 
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetHoldersAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<Holder>)new List<Holder>().AsReadOnly();
                }
                return (IReadOnlyList<Holder>)t.Result.AsReadOnly();
            }, ct);
        }

        public Task<IReadOnlyList<Holding>> GetHoldingsAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                var db = GetRows("SELECT * FROM HHF_Holdings WHERE AsOfDate = #" + asOfDate.ToString("yyyyMMdd") + "#");
                var list = new List<Holding>();
                foreach (DataRow row in db.Rows)
                {
                    // Check for cancellation - exit gracefully without throwing
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine($"GetHoldingsAsync: Cancellation requested. Returning {list.Count} values retrieved so far.");
                        break;
                    }
                    list.Add(row.Hydrate<Holding>());
                }
                return list;
            }, ct).ContinueWith(t => 
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetHoldingsAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<Holding>)new List<Holding>().AsReadOnly();
                }
                return (IReadOnlyList<Holding>)t.Result.AsReadOnly();
            }, ct);
        }        

        public Task<IReadOnlyList<PresentValue>> GetPresentValuesAsync(DateTime asOfDate, IList<string> CusipOrSymbolList, SeekOrigin scope, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                var symbols = string.Join(",", CusipOrSymbolList.Select(s => $"'{s.Replace("'", "''")}'"));

                var sql =
                    $"SELECT * FROM HHF_PresentValue " +
                    $"WHERE AsOfDate <= #{asOfDate:yyyyMMdd}# " +
                    $"AND CusipOrSymbol IN ({symbols})";
                var db = GetRows(sql);
                var list = new List<PresentValue>();
                foreach (DataRow row in db.Rows)
                {
                    // Check for cancellation - exit gracefully without throwing
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine($"GetPresentValuesAsync: Cancellation requested. Returning {list.Count} values retrieved so far.");
                        break;
                    }
                    list.Add(row.Hydrate<PresentValue>());
                }
                return list;
            }, ct).ContinueWith(t =>
            {
                // Handle task cancellation gracefully
                if (t.IsCanceled)
                {
                    Console.WriteLine("GetPresentValuesAsync: Task was cancelled. Returning empty list.");
                    return (IReadOnlyList<PresentValue>)new List<PresentValue>().AsReadOnly();
                }
                return (IReadOnlyList<PresentValue>)t.Result.AsReadOnly();
            }, ct);
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

        public Task<IReadOnlyList<HolderHoldingTransaction>> GetTransactionsAsync(DateTime asOfDate, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                var db = GetRows("SELECT * FROM HHF_Transactions WHERE AsOfDate = #" + asOfDate.ToString("yyyyMMdd") + "#");
                var list = new List<HolderHoldingTransaction>();
                foreach (DataRow row in db.Rows)
                {
                    list.Add(row.Hydrate<HolderHoldingTransaction>());
                }                
                return list;
            }).ContinueWith(t => (IReadOnlyList<HolderHoldingTransaction>)t.Result.AsReadOnly());
        }

        #region Test-only Active Data Object Helpers
        public DataTable GetRows(string sql)
        {
            DataTable dt = new DataTable();
            using (var connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var command = new OleDbCommand(sql, connection))
                    {
                        command.CommandTimeout = 30;
                        using (var adapter = new OleDbDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill DataTable with query results
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error for query {sql}: {ex.Message}");
                }
            }
            return dt;
        }

        public void SetRowKeyVal(DataRow row, string key, string val)
        {
            if (row != null && row.Table.Columns.Contains(key))
            {
                var col = row.Table.Columns[key];
                if (col.DataType.Name.ToLower().Contains("dec"))
                {
                    if (Decimal.TryParse(val, out Decimal decimalVal))
                    {
                        row[key] = decimalVal;
                    }
                }
                else if (col.DataType.Name.ToLower().Contains("date"))
                {
                    if (DateTime.TryParse(val, out DateTime dateVal))
                    {
                        row[key] = dateVal;
                    }
                }
                else
                {
                    row[key] = val;
                }
            }
        }
        public int UpdateRow(DataRow row, string sql)
        {
            // Update or insert the Holder
            int result = 0;
            if (row != null)
            {
                var dt = row.Table;
                if (row.RowState == DataRowState.Detached)
                {
                    dt.Rows.Add(row);
                }
                else if (row.RowState == DataRowState.Unchanged)
                {
                    return 0;
                }
                result = UpdateTable(sql, dt);
            }
            return result;
        }

        public int UpdateTable(string sql, DataTable dt)
        {
            int result;
            // Use OleDbCommandBuilder to generate the appropriate SQL for insert/update
            using (var connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var command = new OleDbCommand(sql, connection))
                    {
                        command.CommandTimeout = 30;
                        using (var adapter = new OleDbDataAdapter(command))
                        {
                            using (var builder = new OleDbCommandBuilder(adapter))
                            {
                                result = adapter.Update(dt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error for query {sql}: {ex.Message}");
                    result = -1;
                }
            }

            return result;
        }
        #endregion

        public IList<string> UnzipResourceToTemp(string resourceName, string tempSubFolder = "")
        {
            // 1. Get a unique temporary path
            var found = new List<string>();
            string tempDirectory = Path.Combine(Path.GetTempPath(), tempSubFolder);
            Directory.CreateDirectory(tempDirectory);

            // 2. Access the embedded resource stream from the executing assembly
            var assm = typeof(HolderHoldingsFlowTest).Assembly;
            var resourceFullName = assm.GetManifestResourceNames().FirstOrDefault(n => n.Contains(resourceName));
            if (string.IsNullOrEmpty(resourceFullName)) return new string[] { string.Empty };
            using (Stream resourceStream = assm.GetManifestResourceStream(resourceFullName))
            {
                if (resourceStream == null)
                    throw new FileNotFoundException($"Resource not found: {resourceName}");

                // 3. Open the stream in a ZipArchive
                using (var archive = new ZipArchive(resourceStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Prevent path traversal attacks (e.g., zip files containing "../")
                        string destinationPath = Path.GetFullPath(Path.Combine(tempDirectory, entry.FullName));

                        // Ensure the extracted file stays within the target directory
                        if (!destinationPath.StartsWith(tempDirectory, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(entry.Name))
                        {
                            // It's a directory, just create it
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            // It's a file, ensure the directory exists and extract
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                            if (!File.Exists(destinationPath))
                            {
                                entry.ExtractToFile(destinationPath, overwrite: false);
                            }
                            found.Add(destinationPath);
                        }
                    }
                }
            }
            return found;
        }
    }
