@echo off
REM Helper script to run Liquibase migrations with environment-specific config

setlocal enabledelayedexpansion

set ENVIRONMENT=%1
if "%ENVIRONMENT%"=="" set ENVIRONMENT=local

set ACTION=%2
if "%ACTION%"=="" set ACTION=update

if "%ENVIRONMENT%"=="local" (
    set PROPS_FILE=liquibase.properties
) else if "%ENVIRONMENT%"=="staging" (
    set PROPS_FILE=liquibase.properties.staging
) else if "%ENVIRONMENT%"=="prod" (
    set PROPS_FILE=liquibase.properties.prod
) else (
    echo Usage: migrate.bat [local^|staging^|prod] [update^|status^|rollback]
    exit /b 1
)

echo Running Liquibase %ACTION% on %ENVIRONMENT% environment...
echo Using config: %PROPS_FILE%

mvn liquibase:%ACTION% -Dliquibase.propertyFile=%PROPS_FILE%

if %errorlevel% equ 0 (
    echo ✅ Migration complete!
) else (
    echo ❌ Migration failed!
    exit /b 1
)
