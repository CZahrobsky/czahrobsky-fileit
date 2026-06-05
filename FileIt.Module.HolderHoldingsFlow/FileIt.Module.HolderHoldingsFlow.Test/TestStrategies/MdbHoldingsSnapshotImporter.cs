using System;
using System.Data;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Test.TestStrategies;

public class MdbHoldingsSnapshotImporter : TestSource, IHoldingsSnapshotImporter
{
    public Task ImportSnapshotAsync(Stream file, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            long lineNumber = 0;
            var sqlHolding = $"SELECT TOP 1 * FROM HHF_Holdings WHERE HolderId = 'N/A'";
            var dtHolding = GetRows(sqlHolding);

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
                        DateTime test;
                        if (!DateTime.TryParse(row["AsOfDate"].ToString(), out test))
                        {
                            row["AsOfDate"] = DateTime.Today;
                        }
                        UpdateRow(row, sqlHolding);
                    }
                }
            }
        }, ct);
    }
}
