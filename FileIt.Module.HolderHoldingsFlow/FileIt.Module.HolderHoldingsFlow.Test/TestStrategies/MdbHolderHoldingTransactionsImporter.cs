using System;
using System.Data;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Test.TestStrategies;
using static System.Net.Mime.MediaTypeNames;

public class MdbHolderHoldingTransactionsImporter : TestSource, IHolderHoldingTransactionsImporter
{
    public Task ImportHolderHoldingTransactionsAsync(Stream file, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            long lineNumber = 0;
            var sqlTransactions = $"SELECT TOP 1 * FROM HHF_Transactions WHERE HolderId = 'N/A'";
            var txList = GetRows(sqlTransactions);

            // Load manifest resource from assembly into string
            string line = null;
            string id = null;
            string sql = null;
            var dt = new DataTable();
            DataRow row = null;
            string[] jsonLines = null;

            using (var reader = new StreamReader(file))
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
                        txList.Rows.Clear();
                        row = txList.NewRow();
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
                        DateTime test;
                        if (!DateTime.TryParse(row["AsOfDate"].ToString(), out test))
                        {
                            row["AsOfDate"] = DateTime.Today;
                        }
                        UpdateRow(row, sqlTransactions);
                    }
                }
            }
        }, ct);
    }
}
