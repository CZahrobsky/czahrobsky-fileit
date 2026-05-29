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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

[TestClass]
public class HolderHoldingsFlowTest
{
    static string mdbFile = @"D:\Source\Hackathon\FileIt\cz-develop\FileIt.Module.HolderHoldingsFlow\FileIt.Module.HolderHoldingsFlow.Integration\FileIt.dat";
    static string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={mdbFile};";

    [TestMethod]
    public void TestHolders()
    {
        HolderIngestionService holderIngest = new HolderIngestionService();
        HolderIngestionService.Source = new TestSource();
        var holders = holderIngest.LoadHoldersAsync().GetAwaiter().GetResult();

    }

    [TestMethod]
    public void ImportIntegrationDataResources()
    {
        char qt = '\"';
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
                if (res.ToLower().Contains("holder"))
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
                                if (line.Replace("\t", "").Trim().EndsWith("}"))
                                {
                                    UpdateRow(row, sql);
                                }
                            }
                        }
                    }
                }
                if (res.ToLower().Contains("holding"))
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
                                    dtHolding.Rows.Clear();
                                    row = dtHolding.NewRow();
                                    row["HolderId"] = id;
                                }
                                else
                                {
                                    SetRowKeyVal(row, key, val);
                                }
                                if (line.Replace("\t", "").Trim().EndsWith("}"))
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
        DataTable dt = null;
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
            else
            {
                row.SetModified();
            }
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
        }
        return result;
    }
}

