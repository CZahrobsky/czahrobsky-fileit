using System;
using System.Data;
using FileIt.Infrastructure.Data;
using FileIt.Module.HolderHoldingsFlow.App.Strategies;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using FileIt.Module.HolderHoldingsFlow.Host.Data;

namespace FileIt.Module.HolderHoldingsFlow.Host.Infrastructure;

    public class SqlManagedInstanceHoldingsSnapshotImporter : IHoldingsSnapshotImporter
    {
        private readonly HolderHoldingsDbContext context;
        static char qt = '\"';
        public SqlManagedInstanceHoldingsSnapshotImporter(HolderHoldingsDbContext context)
        {
            this.context = context;
        }

        public Task ImportSnapshotAsync(Stream file, CancellationToken ct)
        {
            return Task.Run(async () =>
            {
                long lineNumber = 0;
                var holdingsList = context.Holdings.OrderByDescending(h => h.AsOfDate).Take(1).ToList();
                var maxDate = holdingsList.Count > 0 ? holdingsList[0].AsOfDate : DateTime.Today;

                // Load manifest resource from assembly into string
                string line = null;
                string id = null;
                string sql = null;
                var dt = new DataTable();
                Holding holding = null;
                string[] jsonLines = null;

                using (var reader = new StreamReader(file))
                {
                    while (reader != null && (line = reader.ReadLine()) != null)
                    {
                        lineNumber++;
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string key = (line + qt + qt + qt).Split(qt)[1];
                        string val = (line + qt + qt + qt).Split(qt)[3];

                        if (key.ToLower() == "holderid")
                        {
                            id = val;
                            holdingsList.Clear();
                            holding = new Holding();
                            holding.HolderId = id;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(val) && line.Contains(":"))
                            {
                                val = line.Split(':').Last().Replace(",", "").Replace(qt + "", "").Trim();
                            }
                            holding.HydrateKeyValue(key, val);
                        }

                        if (line.Replace("\t", "").Replace(",", "").Trim().EndsWith("}"))
                        {
                            if (holding.AsOfDate == DateTime.MinValue)
                            {
                                holding.HydrateKeyValue("AsOfDate", DateTime.Today.ToString());
                            }

                            // Use GetExisting extension to find duplicates matching all fields except *Id
                            var existingHolding = context.Holdings.GetExisting(holding);

                            if (existingHolding == null)
                            {
                                // Add new record
                                context.Holdings.Add(holding);
                                Console.WriteLine($"Added new holding for HolderId: {holding.HolderId}, AsOfDate: {holding.AsOfDate}");
                            }

                            // Save changes to database
                            await context.SaveChangesAsync(ct).ConfigureAwait(false);
                        }
                    }
                }
            }, ct);
        }
    }
