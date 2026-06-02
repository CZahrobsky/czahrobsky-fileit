using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using FileIt.Infrastructure;
using FileIt.Infrastructure.Extensions;
using FileIt.Infrastructure.Logging;
using FileIt.Infrastructure.Middleware;
using FileIt.Module.HolderHoldingsFlow.App.Services;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Test;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileIt.Module.HolderHoldingsFlow.Test
{
    [TestClass]
    public class HolderHoldingsFlowTest
    {
        static string mdbFile = "";
        static string connectionString = "";
        static char qt = '\"';

        [TestInitialize]
        public void TestInitialize()
        {
            // Unzip the embedded MDB file resource to a temporary location for testing
            mdbFile = UnzipResourceToTemp("FileIt.zip").FirstOrDefault() ?? "N/A";
            Assert.IsTrue(File.Exists(mdbFile), "Failed to extract MDB file for testing.");
            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={mdbFile};";
        }

        [TestMethod]
        public void TestHolders()
        {
            HolderIngestionService holderIngest = new HolderIngestionService();
            HolderIngestionService.Source = new TestSource();
            var holders = holderIngest.LoadHoldersAsync().GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ImportPresentValues()
        {
            var toProcess = UnzipResourceToTemp("PresentValues");
            var sqlValues = $"SELECT * FROM HHF_PresentValue WHERE AsOfDate > (SELECT MAX(AsOfDate - 3) FROM HHF_PresentValue) ORDER BY ID";
            var dtValues = GetRows(connectionString, sqlValues);
            Assert.IsNotNull(dtValues);
            long lastCount = dtValues.Rows.Count;
            DateTime parsedDate = DateTime.MinValue;
            DateTime minImport = lastCount > 0 && DateTime.TryParse(dtValues.Rows[0]["AsOfDate"] + "",
                     out parsedDate) ? parsedDate : DateTime.MinValue;
            string line = null;
            foreach (var file in toProcess ?? Array.Empty<string>())
            {
                string valFile = file + "";
                if (File.Exists(valFile) && valFile.ToLower().Contains("val") && valFile.ToLower().EndsWith(".json"))
                {
                    using (var stream = new FileStream(valFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                var row = dtValues.NewRow();
                                if (line.Split(qt).Any(d => DateTime.TryParse(d, out parsedDate)) && parsedDate < minImport)
                                {
                                    continue;
                                }
                                var pairs = line.Substring(line.IndexOf(qt) + 1).Replace("," + qt, "\t").Replace("},", "\t")
                                    .Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var pair in pairs)
                                {
                                    string key = pair.Split(qt)[0];
                                    string val = (pair + ":").Split(':')[1].Replace(qt + "", "");
                                    SetRowKeyVal(row, key, val);
                                }
                                if (row[1] + "" == "") continue;
                                string where = "CusipOrSymbol='" + row["CusipOrSymbol"];
                                where += "' AND DividendMultiple=" + (row["DividendMultiple"] + "" == "" ? "1" : row["DividendMultiple"]);
                                where += " AND SplitMultiple=" + (row["SplitMultiple"] + "" == "" ? "1" : row["SplitMultiple"]);
                                where += " AND AsOfDate='" + row["AsOfDate"] + "'";
                                var dv = new DataView(dtValues, where, "", DataViewRowState.CurrentRows);
                                if (dv.Count < 1 && DateTime.TryParse(row["AsOfDate"] + "", out parsedDate) && parsedDate >= minImport)
                                {
                                    dtValues.Rows.Add(row);
                                    if (dtValues.Rows.Count > lastCount + 1001)
                                    {
                                        lastCount = dtValues.Rows.Count;
                                        UpdateTable(sqlValues, dtValues);
                                    }
                                }
                            }
                        }
                        if (dtValues.Rows.Count > lastCount)
                        {
                            UpdateTable(sqlValues, dtValues);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ImportIntegrationDataResources()
        {
            Assert.IsNotNull(qt, "ImportIntegrationDataResources failed");
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // Only run this test when a debugger is attached to import data locally
                return;
            }
            var assm = typeof(HolderHoldingsFlowTest).Assembly;
            var sqlHolding = $"SELECT TOP 1 * FROM HHF_Holdings WHERE HolderId = 'N/A'";
            var dtHolding = GetRows(connectionString, sqlHolding);
            Assert.IsNotNull(dtHolding);
            foreach (var res in assm.GetManifestResourceNames())
            {
                // Load manifest resource from assembly into string
                string line = null;
                string id = null;
                string sql = null;
                long lineNumber = 0;
                var dt = new DataTable();
                DataRow row = null;
                string[] jsonLines = null;
                if (res.EndsWith(".json"))
                {
                    // Process the .json resource
                    if (res.ToLower().Replace(".json", "").Split('.').Last().Contains("holder"))
                    {
                        using (var stream = assm.GetManifestResourceStream(res))
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                while ((line = reader.ReadLine()) != null)
                                {
                                    lineNumber++;
                                    if (string.IsNullOrWhiteSpace(line))
                                        continue;
                                    string key = (line + qt + qt + qt).Split(qt)[1];
                                    string val = (line + qt + qt + qt).Split(qt)[3];
                                    if (key.ToLower() == "holderid")
                                    {
                                        id = val;
                                        sql = $"SELECT TOP 1 * FROM HHF_Holders WHERE HolderId = '{id}'";
                                        // Connect to the local OLEDB database and populate DataTable
                                        dt = GetRows(connectionString, sql);
                                        Assert.IsNotNull(dt);
                                        row = dt == null ? null : (dt.Rows.Count > 0 ? dt.Rows[0] : dt.NewRow());
                                        row["HolderId"] = id;
                                    }
                                    else
                                    {
                                        SetRowKeyVal(row, key, val);
                                    }
                                    if (line.Replace("\t", "").Replace(",", "").Trim().EndsWith("}"))
                                    {
                                        UpdateRow(row, sql);
                                    }
                                }
                            }
                        }
                    }
                    if (res.ToLower().Replace(".json", "").Split('.').Last().Contains("holding"))
                    {
                        using (var stream = assm.GetManifestResourceStream(res))
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                while ((line = reader.ReadLine()) != null)
                                {
                                    lineNumber++;
                                    if (lineNumber > 2500) break; // Limit to first 2500 lines for testing
                                    if (string.IsNullOrWhiteSpace(line))
                                        continue;
                                    string key = (line + qt + qt + qt).Split(qt)[1];
                                    string val = (line + qt + qt + qt).Split(qt)[3];
                                    if (key.ToLower() == "holderid")
                                    {
                                        id = val;
                                        dtHolding.Rows.Clear();
                                        row = dtHolding.NewRow();
                                        row["HolderId"] = id;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(val) && line.Contains(":"))
                                        {
                                            val = line.Split(':').Last().Replace(",", "").Replace(qt + "", "").Trim();
                                        }
                                        SetRowKeyVal(row, key, val);
                                    }
                                    if (line.Replace("\t", "").Replace(",", "").Trim().EndsWith("}"))
                                    {
                                        row["AsOfDate"] = DateTime.Today;
                                        UpdateRow(row, sqlHolding);
                                    }
                                }
                            }
                        }
                    }
                    Assert.IsNotNull(dt);
                }
            }
        }

        private static DataTable GetRows(string connectionString, string sql)
        {
            DataTable dt = new DataTable();
            using (var connection = new OleDbConnection(connectionString))
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

        private static void SetRowKeyVal(DataRow row, string key, string val)
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
        private static int UpdateRow(DataRow row, string sql)
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

        public static IList<string> UnzipResourceToTemp(string resourceName, string tempSubFolder = "")
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

        private static int UpdateTable(string sql, DataTable dt)
        {
            int result;
            // Use OleDbCommandBuilder to generate the appropriate SQL for insert/update
            using (var connection = new OleDbConnection(connectionString))
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
    }
}
