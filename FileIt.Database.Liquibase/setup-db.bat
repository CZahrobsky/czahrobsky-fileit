@echo off
REM PostgreSQL FileIt Database Setup Script for Windows

setlocal enabledelayedexpansion

echo 🐘 FileIt PostgreSQL Database Setup
echo ====================================

REM Configuration
set DB_HOST=localhost
set DB_PORT=5432
set DB_USER=postgres
set DB_NAME=fileit

REM Check if psql is available
where psql >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ PostgreSQL is not installed or psql is not in PATH
    echo Download from: https://www.postgresql.org/download/windows/
    exit /b 1
)

echo ✅ PostgreSQL client found

REM Prompt for password
set /p DB_PASSWORD="Enter PostgreSQL password for user '%DB_USER%': "

REM Test connection
echo Testing connection to PostgreSQL...
set PGPASSWORD=%DB_PASSWORD%
psql -h %DB_HOST% -U %DB_USER% -p %DB_PORT% -c "\q" >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Failed to connect to PostgreSQL
    echo Check your credentials and that PostgreSQL is running
    exit /b 1
)

echo ✅ Connected successfully

REM Create database
echo Creating database '%DB_NAME%'...
psql -h %DB_HOST% -U %DB_USER% -p %DB_PORT% -c "CREATE DATABASE %DB_NAME% OWNER %DB_USER% ENCODING 'UTF8';" >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Database created
) else (
    echo ⚠️  Database may already exist
)

REM Verify
echo.
echo Available databases:
psql -h %DB_HOST% -U %DB_USER% -p %DB_PORT% -l | find "%DB_NAME%"

echo.
echo ✅ Setup complete!
echo.
echo Connection string:
echo    postgresql://%DB_USER%:password@%DB_HOST%:%DB_PORT%/%DB_NAME%

set PGPASSWORD=
