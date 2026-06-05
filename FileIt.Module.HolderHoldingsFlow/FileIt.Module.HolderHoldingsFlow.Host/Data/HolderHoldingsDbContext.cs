using FileIt.Infrastructure.Data;
using FileIt.Module.HolderHoldingsFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileIt.Module.HolderHoldingsFlow.Host.Data;

    public class HolderHoldingsDbContext : CommonDbContext
    {
        public HolderHoldingsDbContext(DbContextOptions options)
                 : base(options) { }

        public DbSet<Holder> Holders { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<HolderHoldingTransaction> HolderHoldingTransactions { get; set; }

        // public DbSet<HolderHoldingValuation> HolderHoldingValuations { get; set; }
        // public DbSet<PortfolioReportRow> PortfolioReportRows { get; set; }

        public DbSet<PresentValue> PresentValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder
                .Entity<Holder>()
                .ToTable("HHF_Holders")
                .Property(s => s.HolderId)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<HolderHoldingTransaction>()
                .ToTable("HHF_Transactions")
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<Holding>()
                .ToTable("HHF_Holdings")
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<PresentValue>()
                .ToTable("HHF_PresentValues")
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();
        }

    }
