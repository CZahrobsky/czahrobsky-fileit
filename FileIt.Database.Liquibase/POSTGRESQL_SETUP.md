# PostgreSQL Setup Guide for FileIt

Complete instructions for installing PostgreSQL and creating a local FileIt database.

## Quick Start (Windows)

### 1. Install PostgreSQL

**Via Chocolatey** (Recommended):
```powershell
# Install PostgreSQL 16
choco install postgresql

# Verify
psql --version
```

**Via Installer** (Manual):
1. Download: https://www.postgresql.org/download/windows/
2. Run installer
3. **Save the password you enter** (you'll need it)
4. Accept default port: **5432**

### 2. Create FileIt Database

**Option A: Using Command Line** (Fastest)
```powershell
# Run the setup script
cd FileIt.Database.Liquibase
.\setup-db.bat
```

**Option B: Manual via psql**
```powershell
# Connect to PostgreSQL
psql -U postgres -h localhost

# In the psql prompt (psql=#), run:
CREATE DATABASE fileit OWNER postgres ENCODING 'UTF8';

# Verify
\l

# Exit
\q
```

**Option C: Using pgAdmin** (GUI)
1. Open **pgAdmin 4** (installed with PostgreSQL)
2. Right-click **Databases** → **Create** → **Database**
3. Name: `fileit`
4. Owner: `postgres`
5. Click **Save**

### 3. Update Liquibase Configuration

Edit `liquibase.properties`:
```properties
driver=org.postgresql.Driver
url=jdbc:postgresql://localhost:5432/fileit
username=postgres
password=YourPostgresPassword
changeLogFile=./src/main/resources/db/changelog/db.changelog-master.sql
logLevel=info
```

### 4. Run Migrations

```powershell
cd FileIt.Database.Liquibase

# Download dependencies and apply migrations
mvn clean liquibase:update

# Verify status
mvn liquibase:status

# Check tables in PostgreSQL
psql -U postgres -h localhost -d fileit -c "\dt"
```

---

## Detailed Installation Steps

### Windows Installation

#### Method 1: Chocolatey (Easiest)

```powershell
# Open PowerShell as Administrator

# If you don't have Chocolatey, install it first:
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Install PostgreSQL
choco install postgresql --params '/Password:StrongPassword123'

# Verify installation
psql --version

# Add to PATH (if needed)
$env:Path += ";C:\Program Files\PostgreSQL\16\bin"
```

#### Method 2: Official Installer

1. Download from: https://www.postgresql.org/download/windows/
2. Run `postgresql-16.x-x64.exe`
3. Follow installer:
   - **Installation Directory**: `C:\Program Files\PostgreSQL\16`
   - **Port**: `5432` (default)
   - **Superuser Password**: `YourStrongPassword123` (write this down!)
   - **Locale**: `[Default locale]`
4. Click **Install**
5. Finish installation

#### Method 3: Windows Package Manager

```powershell
# Requires Windows 11 or Windows 10 with App Installer
winget install PostgreSQL.PostgreSQL

# Follow prompts for password and configuration
```

#### Verify Installation

```powershell
# Check PostgreSQL version
psql --version

# Start PostgreSQL service (if not running)
Get-Service postgresql-x64-16 | Start-Service

# Test connection
psql -U postgres -h localhost -c "SELECT version();"
```

### Mac Installation

```bash
# Using Homebrew (recommended)
brew install postgresql@16

# Start PostgreSQL
brew services start postgresql@16

# Verify
psql --version

# Create superuser (if needed)
createuser -P postgres
```

### Linux Installation

#### Ubuntu/Debian
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib

# Start service
sudo systemctl start postgresql

# Verify
psql --version
```

#### Red Hat/CentOS
```bash
sudo yum install postgresql-server postgresql-contrib

# Initialize database
sudo postgresql-setup initdb

# Start service
sudo systemctl start postgresql

# Verify
psql --version
```

---

## Create FileIt Database

### Method 1: Automated Script

```powershell
cd FileIt.Database.Liquibase

# Windows
.\setup-db.bat

# Mac/Linux
./setup-db.sh
```

### Method 2: Manual psql

```powershell
# Open psql prompt
psql -U postgres -h localhost

# You'll see: psql=#

# Create database
CREATE DATABASE fileit 
  OWNER postgres 
  ENCODING 'UTF8' 
  LC_COLLATE 'en_US.UTF-8' 
  LC_CTYPE 'en_US.UTF-8';

# List all databases
\l

# Connect to fileit database
\c fileit

# List tables (should be empty initially)
\dt

# Exit
\q
```

### Method 3: One-Liner

```powershell
# Set password environment variable
$env:PGPASSWORD = "YourPostgresPassword"

# Create database
psql -U postgres -h localhost -c "CREATE DATABASE fileit OWNER postgres ENCODING 'UTF8';"

# Verify
psql -U postgres -h localhost -l | Select-String fileit
```

### Method 4: pgAdmin GUI

1. **Open pgAdmin 4**
   - Windows Start Menu → pgAdmin 4
   - Mac: Applications → pgAdmin 4
   - Or: http://localhost:5050

2. **Login**
   - Email: `postgres@pgadmin.org` (or your configured email)
   - Password: Your pgAdmin password

3. **Create Database**
   - Left panel → **Servers** → **PostgreSQL 16** (may need password)
   - Right-click **Databases** → **Create** → **Database**
   - Name: `fileit`
   - Owner: `postgres`
   - **Save**

4. **Verify**
   - Expand **fileit** → **Schemas** → **public** → **Tables**
   - (Tables will appear after Liquibase runs)

---

## Apply Liquibase Migrations

### Prerequisites

Ensure you have:
- ✅ PostgreSQL running
- ✅ `fileit` database created
- ✅ Java 11+ installed (`java -version`)
- ✅ Maven installed (`mvn -version`)

### Run Migrations

```powershell
cd FileIt.Database.Liquibase

# Step 1: Update liquibase.properties with your password
notepad liquibase.properties

# Step 2: Apply migrations
mvn clean liquibase:update

# Step 3: Verify
mvn liquibase:status
```

**Expected Output:**
```
[INFO] Reading from fileit.xml
[INFO] Executing SQL: select count(*) from databasechangelog
[INFO] Successfully released lock
```

### Verify Tables Created

```powershell
# Connect to fileit database
psql -U postgres -h localhost -d fileit

# List tables
\dt

# Should show:
# Schema |           Name            | Type  | Owner
# --------+---------------------------+-------+----------
#  public | databasechangelog         | table | postgres
#  public | databasechangeloglock     | table | postgres
#  public | hhf_holders               | table | postgres
#  public | hhf_holdings              | table | postgres
#  public | hhf_presentvalue          | table | postgres

# Describe table structure
\d hhf_holders

# Exit
\q
```

---

## Troubleshooting

### "psql: command not found"

**Solution**: PostgreSQL bin directory not in PATH

```powershell
# Add to PATH permanently (Windows)
[Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files\PostgreSQL\16\bin", "User")

# Reload PowerShell
```

### "FATAL: password authentication failed"

**Solutions**:
1. Wrong password entered (default: `postgres`)
2. User doesn't exist
3. Authentication method misconfigured

```powershell
# Reset superuser password (Windows, as admin)
net stop postgresql-x64-16
pg_ctl -D "C:\Program Files\PostgreSQL\16\data" -U postgres -W password

# Restart service
net start postgresql-x64-16

# Try connecting
psql -U postgres -h localhost
```

### "database fileit already exists"

**Solution**: Use `IF NOT EXISTS`

```powershell
psql -U postgres -h localhost -c "DROP DATABASE IF EXISTS fileit; CREATE DATABASE fileit OWNER postgres;"
```

### "Liquibase: Driver class not found: org.postgresql.Driver"

**Solution**: Maven dependency issue

```powershell
cd FileIt.Database.Liquibase

# Clear Maven cache
mvn clean

# Re-download dependencies
mvn install

# Try again
mvn liquibase:update
```

### "Connection to localhost:5432 refused"

**Solution**: PostgreSQL service not running

```powershell
# Windows
Get-Service postgresql-x64-16 | Start-Service

# Mac
brew services start postgresql@16

# Linux
sudo systemctl start postgresql
```

### Cannot connect from .NET application

**Update connection string in your .NET project**:

```csharp
// Npgsql connection string
var connectionString = "Host=localhost;Port=5432;Database=fileit;Username=postgres;Password=YourPassword";
```

Or in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=fileit;Username=postgres;Password=YourPassword"
  }
}
```

---

## Using Different Environments

### Development (Local)

```properties
# liquibase.properties
driver=org.postgresql.Driver
url=jdbc:postgresql://localhost:5432/fileit
username=postgres
password=YourLocalPassword
```

### Staging

```properties
# liquibase.properties.staging
driver=org.postgresql.Driver
url=jdbc:postgresql://staging.example.com:5432/fileit_staging
username=fileit_user
password=${STAGING_DB_PASSWORD}
```

### Production (AWS RDS)

```properties
# liquibase.properties.prod
driver=org.postgresql.Driver
url=jdbc:postgresql://fileit-prod.xxxxx.us-east-1.rds.amazonaws.com:5432/fileit
username=fileit_admin
password=${PROD_DB_PASSWORD}
logLevel=warning
```

### Apply migrations to specific environment

```powershell
# Local
mvn liquibase:update

# Staging
mvn liquibase:update -Dliquibase.propertyFile=liquibase.properties.staging

# Production
mvn liquibase:update -Dliquibase.propertyFile=liquibase.properties.prod
```

---

## Next Steps

1. ✅ Install PostgreSQL
2. ✅ Create `fileit` database
3. ✅ Update `liquibase.properties`
4. ✅ Run `mvn liquibase:update`
5. ✅ Verify tables with `psql`
6. 🔄 **Integrate with .NET 10 application** (next guide)

---

## Resources

- [PostgreSQL Official Downloads](https://www.postgresql.org/download/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [pgAdmin Documentation](https://www.pgadmin.org/docs/)
- [Liquibase PostgreSQL Support](https://docs.liquibase.com/get-started/best-practices/postgresql)
- [Npgsql (.NET PostgreSQL Driver)](https://www.npgsql.org/)
