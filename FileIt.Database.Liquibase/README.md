# FileIt Database - Liquibase Migrations

This project manages database schema migrations for FileIt using **Liquibase**, a version control system for databases.

## Overview

**Why Liquibase?**
- ✅ Version control for database changes
- ✅ Rollback capability for failed deployments
- ✅ Cross-database support (SQL Server, PostgreSQL, MySQL, etc.)
- ✅ Better CI/CD integration
- ✅ Replaces legacy SQL Server Database Projects (.sqlproj)

## Project Structure

```
FileIt.Database.Liquibase/
├── pom.xml                              # Maven configuration
├── liquibase.properties                 # Connection settings (local)
├── liquibase.properties.prod            # Connection settings (production)
├── src/main/resources/db/changelog/
│   ├── db.changelog-master.sql          # Master changelog (orchestrates all migrations)
│   └── v1.0/
│       ├── 01-create-hhf-tables.sql     # Initial HHF table creation
│       └── 02-add-indexes.sql           # Index definitions (future)
└── README.md
```

## Tables

### HHF_Holders
Stores holder/customer information.

| Column | Type | Nullable | Notes |
|--------|------|----------|-------|
| HolderId | VARCHAR(30) | NO | Primary Key |
| CustomerName | NVARCHAR(200) | NO | |
| AccountNumber | VARCHAR(50) | NO | |
| Address | VARCHAR(200) | YES | |
| City | VARCHAR(100) | YES | |
| State | VARCHAR(50) | YES | |
| Zip | VARCHAR(20) | YES | |
| CreatedAt | DATETIME | NO | |
| AccountClosed | DATETIME | YES | |
| UpdatedAt | DATETIME | NO | |

### HHF_Holdings
Stores holdings/securities data.

| Column | Type | Nullable | Notes |
|--------|------|----------|-------|
| Id | INT IDENTITY | NO | Primary Key |
| HolderId | VARCHAR(30) | NO | Foreign Key → HHF_Holders |
| CusipOrSymbol | VARCHAR(50) | NO | |
| Name | VARCHAR(200) | YES | |
| Quantity | DECIMAL(38,32) | NO | |
| AsOfDate | DATETIME | NO | Default: Today |

### HHF_PresentValue
Stores pricing and market data.

| Column | Type | Nullable | Notes |
|--------|------|----------|-------|
| Id | INT IDENTITY | NO | Primary Key |
| CusipOrSymbol | VARCHAR(50) | NO | |
| UnitPrice | MONEY | NO | |
| DividendMultiple | DECIMAL(38,32) | NO | |
| SplitMultiple | DECIMAL(38,32) | NO | |
| CumulativeSplits | DECIMAL(38,32) | NO | |
| RiskScalar | DECIMAL(38,32) | YES | |
| AsOfDate | DATETIME | NO | Default: Today |

## Prerequisites

- **Java** 11 or later (Maven runs on Java)
- **Maven** 3.6+
- **SQL Server** 2019 or later
- **Liquibase CLI** (optional, or use Maven plugin)

## Setup

### 1. Install Prerequisites

```bash
# Check Java
java -version

# Check Maven
mvn -version

# Install Maven (if needed)
# Windows: choco install maven
# Mac: brew install maven
```

### 2. Configure Database Connection

Edit `liquibase.properties`:

```properties
driver=com.microsoft.sqlserver.jdbc.SQLServerDriver
url=jdbc:sqlserver://YOUR_SERVER:1433;databaseName=FileIt;encrypt=true;trustServerCertificate=true
username=sa
password=YourPassword
```

### 3. Run Migrations

```bash
cd FileIt.Database.Liquibase

# Apply all pending migrations
mvn liquibase:update

# Check migration status
mvn liquibase:status

# Rollback to previous state (if needed)
mvn liquibase:rollback -Dliquibase.rollbackCount=1
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Database Migration

on:
  push:
    branches: [main]
    paths:
      - 'FileIt.Database.Liquibase/**'

jobs:
  migrate:
    runs-on: ubuntu-latest
    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2019-latest
        env:
          ACCEPT_EULA: Y
          SA_PASSWORD: StrongPassword123!
        options: >-
          --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q 'SELECT 1'"
          --health-interval=10s
          --health-timeout=3s
          --health-retries=10

    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-java@v3
        with:
          java-version: '11'
      - name: Run Liquibase Migrations
        run: |
          cd FileIt.Database.Liquibase
          mvn liquibase:update \
            -Dliquibase.url="jdbc:sqlserver://sqlserver:1433;databaseName=FileIt" \
            -Dliquibase.username=sa \
            -Dliquibase.password=StrongPassword123!
```

## Adding New Migrations

1. **Create a new changelog file** in `src/main/resources/db/changelog/`:

```sql
--liquibase formatted sql

--changeset fileit:2-add-foreign-keys
ALTER TABLE [dbo].[HHF_Holdings] 
ADD CONSTRAINT FK_HHF_Holdings_HHF_Holders 
FOREIGN KEY ([HolderId]) 
REFERENCES [dbo].[HHF_Holders]([HolderId]);
--rollback ALTER TABLE [dbo].[HHF_Holdings] DROP CONSTRAINT FK_HHF_Holdings_HHF_Holders;
```

2. **Reference it in `db.changelog-master.sql`**:

```sql
--include file="v1.0/02-add-foreign-keys.sql"
```

3. **Apply the migration**:

```bash
mvn liquibase:update
```

## Best Practices

✅ **DO:**
- Use descriptive changeset IDs (e.g., `fileit:2-add-indexes`)
- Always include rollback statements (`--rollback`)
- Test migrations in dev/staging first
- Keep changesets focused (one logical change per changeset)
- Version control the changelog files

❌ **DON'T:**
- Manually modify the `DATABASECHANGELOG` table
- Skip migrations in production
- Use `<modifyDataType>` without a `<rollback>` context
- Store sensitive connection strings in version control (use environment variables)

## Troubleshooting

**"Liquibase connection refused"**
- Verify SQL Server is running: `telnet localhost 1433`
- Check `liquibase.properties` connection settings
- Ensure firewall allows port 1433

**"Changeset already executed"**
- Liquibase tracks executed changesets in `DATABASECHANGELOG` table
- To re-run: Delete row from `DATABASECHANGELOG` and re-apply (`mvn liquibase:update`)

**"Rollback count exceeded"**
- Check `mvn liquibase:rollbackSQL -Dliquibase.rollbackCount=1` first (preview)
- Ensure rollback statements are valid

## Migration from .sqlproj

This Liquibase project replaces the legacy `FileIt.Database.sqlproj`:

| Aspect | .sqlproj | Liquibase |
|--------|----------|-----------|
| Version Control | No | ✅ Yes (Git-friendly) |
| Rollback | Manual | ✅ Automated |
| Cross-DB Support | SQL Server only | ✅ 20+ databases |
| CI/CD Ready | Limited | ✅ Excellent |
| Idempotency | No | ✅ Yes |

## Resources

- [Liquibase Documentation](https://docs.liquibase.com)
- [SQL Server Best Practices](https://docs.liquibase.com/get-started/best-practices/sql-server)
- [Maven Plugin Guide](https://docs.liquibase.com/tools-integrations/maven/home.html)
