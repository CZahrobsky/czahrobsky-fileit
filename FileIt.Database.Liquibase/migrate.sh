#!/bin/bash
# Helper script to run Liquibase migrations with environment-specific config
# Automatically creates the database if it doesn't exist

set -e

ENVIRONMENT=${1:-local}
ACTION=${2:-update}

case $ENVIRONMENT in
  local)
    PROPS_FILE="liquibase.properties"
    DB_HOST="127.0.0.1"
    DB_PORT="5432"
    DB_NAME="fileit"
    DB_USER="postgres"
    ;;
  staging)
    PROPS_FILE="liquibase.properties.staging"
    DB_HOST="staging.example.com"
    DB_PORT="5432"
    DB_NAME="fileit_staging"
    DB_USER="postgres"
    ;;
  prod)
    PROPS_FILE="liquibase.properties.prod"
    DB_HOST="prod.example.com"
    DB_PORT="5432"
    DB_NAME="fileit"
    DB_USER="fileit_admin"
    ;;
  *)
    echo "Usage: ./migrate.sh [local|staging|prod] [update|status|rollback]"
    exit 1
    ;;
esac

echo
echo "============================================"
echo "FileIt Database Migration"
echo "============================================"
echo "Environment: $ENVIRONMENT"
echo "Action: $ACTION"
echo "Config: $PROPS_FILE"
echo

# Check if we need to create the database (for local development)
if [ "$ENVIRONMENT" = "local" ]; then
    echo "Checking if database '$DB_NAME' exists..."

    # Extract password from properties file
    DB_PASSWORD=$(grep "^password=" "$PROPS_FILE" | cut -d'=' -f2 | tr -d ' ')

    if [ -z "$DB_PASSWORD" ]; then
        read -s -p "Enter PostgreSQL password for user '$DB_USER': " DB_PASSWORD
        echo
    fi

    # Check if database exists
    if PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -U "$DB_USER" -p "$DB_PORT" -t -c "SELECT 1 FROM pg_database WHERE datname='$DB_NAME';" 2>/dev/null | grep -q 1; then
        echo "✅ Database '$DB_NAME' already exists."
    else
        echo
        echo "⚠️  Database '$DB_NAME' does not exist. Creating it now..."
        echo

        if PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -U "$DB_USER" -p "$DB_PORT" \
            -c "CREATE DATABASE $DB_NAME OWNER $DB_USER ENCODING 'UTF8' LC_COLLATE 'en_US.UTF-8' LC_CTYPE 'en_US.UTF-8';" 2>/dev/null; then
            echo "✅ Database '$DB_NAME' created successfully."
        else
            echo "❌ Failed to create database '$DB_NAME'."
            exit 1
        fi
    fi

    echo
fi

echo "Running Liquibase $ACTION on $ENVIRONMENT environment..."
echo "Using config: $PROPS_FILE"
echo

mvn liquibase:$ACTION -Dliquibase.propertyFile=$PROPS_FILE

if [ $? -eq 0 ]; then
    echo
    echo "✅ Migration complete!"
    echo
else
    echo
    echo "❌ Migration failed!"
    echo
    exit 1
fi
