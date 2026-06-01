@echo off
REM Helper script to run Liquibase migrations with environment-specific config
REM Automatically creates the database if it doesn't exist

setlocal enabledelayedexpansion

set ENVIRONMENT=%1
if "%ENVIRONMENT%"=="" set ENVIRONMENT=local

set ACTION=%2
if "%ACTION%"=="" set ACTION=update

if "%ENVIRONMENT%"=="local" (
    set PROPS_FILE=liquibase.properties
    set DB_HOST=127.0.0.1
    set DB_PORT=5432
    set DB_NAME=fileit
    set DB_USER=postgres
) else if "%ENVIRONMENT%"=="staging" (
    set PROPS_FILE=liquibase.properties.staging
    set DB_HOST=staging-server
    set DB_PORT=5432
    set DB_NAME=fileit_staging
    set DB_USER=sa
) else if "%ENVIRONMENT%"=="prod" (
    set PROPS_FILE=liquibase.properties.prod
    set DB_HOST=YOUR_SERVER.database.windows.net
    set DB_PORT=5432
    set DB_NAME=fileit
    set DB_USER=fileit_admin
) else (
    echo Usage: migrate.bat [local^|staging^|prod] [update^|status^|rollback]
    exit /b 1
)

echo.
echo ============================================
echo FileIt Database Migration
echo ============================================
echo Environment: %ENVIRONMENT%
echo Action: %ACTION%
echo Config: %PROPS_FILE%
echo.

REM Check if we need to create the database (for local development)
if "%ENVIRONMENT%"=="local" (
    echo Checking if database '%DB_NAME%' exists...

    REM Set password from properties file or prompt
    for /f "tokens=2 delims==" %%A in (findstr /I "^password=" %PROPS_FILE%) do set DB_PASSWORD=%%A

    if "!DB_PASSWORD!"=="" (
        set /p DB_PASSWORD="Enter PostgreSQL password: "
    )

    REM Check if database exists
    set PGPASSWORD=!DB_PASSWORD!

    psql -h %DB_HOST% -U %DB_USER% -p %DB_PORT% -t -c "SELECT 1 FROM pg_database WHERE datname='%DB_NAME%';" 2>nul | findstr /R "^.*1" >nul

    if !errorlevel! neq 0 (
        echo.
        echo ⚠️  Database '%DB_NAME%' does not exist. Creating it now...
        echo.

        psql -h %DB_HOST% -U %DB_USER% -p %DB_PORT% ^
            -c "CREATE DATABASE %DB_NAME% OWNER %DB_USER% ENCODING 'UTF8' LC_COLLATE 'en_US.UTF-8' LC_CTYPE 'en_US.UTF-8';" 2>nul

        if !errorlevel! equ 0 (
            echo ✅ Database '%DB_NAME%' created successfully.
        ) else (
            echo ❌ Failed to create database '%DB_NAME%'.
            exit /b 1
        )
    ) else (
        echo ✅ Database '%DB_NAME%' already exists.
    )

    set PGPASSWORD=
    echo.
)

echo Running Liquibase %ACTION% on %ENVIRONMENT% environment...
echo Using config: %PROPS_FILE%
echo.

mvn liquibase:%ACTION% -Dliquibase.propertyFile=%PROPS_FILE%

if %errorlevel% equ 0 (
    echo.
    echo ✅ Migration complete!
    echo.
) else (
    echo.
    echo ❌ Migration failed!
    echo.
    exit /b 1
)

