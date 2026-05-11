#!/bin/bash
# Helper script to run Liquibase migrations with environment-specific config

set -e

ENVIRONMENT=${1:-local}
ACTION=${2:-update}

case $ENVIRONMENT in
  local)
    PROPS_FILE="liquibase.properties"
    ;;
  staging)
    PROPS_FILE="liquibase.properties.staging"
    ;;
  prod)
    PROPS_FILE="liquibase.properties.prod"
    ;;
  *)
    echo "Usage: ./migrate.sh [local|staging|prod] [update|status|rollback]"
    exit 1
    ;;
esac

echo "Running Liquibase $ACTION on $ENVIRONMENT environment..."
echo "Using config: $PROPS_FILE"

mvn liquibase:$ACTION -Dliquibase.propertyFile=$PROPS_FILE

echo "✅ Migration complete!"
