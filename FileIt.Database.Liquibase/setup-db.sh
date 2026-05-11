#!/bin/bash
# PostgreSQL FileIt Database Setup Script for Linux/Mac

set -e

echo "🐘 FileIt PostgreSQL Database Setup"
echo "===================================="

# Configuration
DB_HOST=${1:-localhost}
DB_PORT=${2:-5432}
DB_USER=${3:-postgres}
DB_NAME="fileit"
DB_PASSWORD=""

# Prompt for password
read -s -p "Enter PostgreSQL password for user '$DB_USER': " DB_PASSWORD
echo

# Check if psql is available
if ! command -v psql &> /dev/null; then
    echo "❌ PostgreSQL is not installed. Please install it first:"
    echo "   Mac: brew install postgresql"
    echo "   Linux: sudo apt-get install postgresql-client"
    exit 1
fi

echo "✅ PostgreSQL client found"

# Test connection
echo "Testing connection to PostgreSQL..."
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER -p $DB_PORT -c "\q" 2>/dev/null || {
    echo "❌ Failed to connect to PostgreSQL"
    echo "Check your credentials and PostgreSQL is running"
    exit 1
}

echo "✅ Connected successfully"

# Create database
echo "Creating database '$DB_NAME'..."
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER -p $DB_PORT -c "CREATE DATABASE $DB_NAME OWNER $DB_USER ENCODING 'UTF8' LC_COLLATE 'en_US.UTF-8' LC_CTYPE 'en_US.UTF-8';" 2>/dev/null || {
    echo "⚠️  Database may already exist (or error occurred)"
}

echo "✅ Database ready"

# List databases
echo ""
echo "Available databases:"
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER -p $DB_PORT -l | grep -E "fileit|postgres"

echo ""
echo "✅ Setup complete!"
echo ""
echo "Connection string:"
echo "   postgresql://$DB_USER:password@$DB_HOST:$DB_PORT/$DB_NAME"
