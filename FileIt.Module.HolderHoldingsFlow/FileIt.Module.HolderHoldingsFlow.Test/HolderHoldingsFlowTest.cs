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
using FileIt.Module.HolderHoldingsFlow.Test.TestStrategies;
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
        static string connectionString = "";
        static char qt = '\"';
        TestSource src = null;

        [TestInitialize]
        public void TestInitialize()
        {
            src = new TestSource();
        }

        [TestMethod]
        public void TestHolders()
        {
            HolderIngestionService holderIngest = new HolderIngestionService();
            HolderIngestionService.Source = src;
            var holders = holderIngest.LoadHoldersAsync().GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ImportPresentValues()
        {
            var toProcess = src.UnzipResourceToTemp("PresentValues");
            var sqlValues = $"SELECT * FROM HHF_PresentValue WHERE AsOfDate > (SELECT MAX(AsOfDate - 3) FROM HHF_PresentValue) ORDER BY ID";
            var dtValues = src.GetRows(sqlValues);
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
                                    src.SetRowKeyVal(row, key, val);
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
                                        src.UpdateTable(sqlValues, dtValues);
                                    }
                                }
                            }
                        }
                        if (dtValues.Rows.Count > lastCount)
                        {
                            src.UpdateTable(sqlValues, dtValues);
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
            var dtHolding = src.GetRows(sqlHolding);
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
                                        dt = src.GetRows(sql);
                                        Assert.IsNotNull(dt);
                                        row = dt == null ? null : (dt.Rows.Count > 0 ? dt.Rows[0] : dt.NewRow());
                                        row["HolderId"] = id;
                                    }
                                    else
                                    {
                                        src.SetRowKeyVal(row, key, val);
                                    }
                                    if (line.Replace("\t", "").Replace(",", "").Trim().EndsWith("}"))
                                    {
                                        src.UpdateRow(row, sql);
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
                                        src.SetRowKeyVal(row, key, val);
                                    }
                                    if (line.Replace("\t", "").Replace(",", "").Trim().EndsWith("}"))
                                    {
                                        row["AsOfDate"] = DateTime.Today;
                                        src.UpdateRow(row, sqlHolding);
                                    }
                                }
                            }
                        }
                    }
                    Assert.IsNotNull(dt);
                }
            }
        }



    }
}
