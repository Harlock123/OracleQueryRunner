#!/bin/bash

# LAWgrid Database Management Script
# Helper script to manage all Docker database environments

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
}

# Function to start all databases
start_all() {
    print_status "Starting all databases..."

    # Start Oracle
    print_status "Starting Oracle..."
    docker-compose up -d

    # Start PostgreSQL
    print_status "Starting PostgreSQL..."
    (cd docker-postgres && docker-compose up -d)

    # Start MySQL
    print_status "Starting MySQL..."
    (cd docker-mysql && docker-compose up -d)

    # Start MongoDB
    print_status "Starting MongoDB..."
    (cd docker-mongo && docker-compose up -d)

    # Start DB2
    print_status "Starting DB2..."
    (cd docker-db2 && docker-compose up -d)

    print_success "All databases started!"
    print_warning "DB2 may take 3-5 minutes to initialize."
    print_warning "Remember to initialize DB2 schema manually:"
    echo "  docker exec lawgrid-db2-db su - db2inst1 -c \"db2 -tvf /var/custom/01-create-schema.sql\""
}

# Function to stop all databases
stop_all() {
    print_status "Stopping all databases..."

    docker-compose stop
    (cd docker-postgres && docker-compose stop)
    (cd docker-mysql && docker-compose stop)
    (cd docker-mongo && docker-compose stop)
    (cd docker-db2 && docker-compose stop)

    print_success "All databases stopped!"
}

# Function to restart all databases
restart_all() {
    print_status "Restarting all databases..."
    stop_all
    sleep 2
    start_all
}

# Function to check status of all databases
status_all() {
    print_status "Checking status of all databases...\n"
    docker ps --filter "name=lawgrid" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
}

# Function to remove all databases (keep volumes)
down_all() {
    print_warning "Removing all database containers (keeping data)..."

    docker-compose down
    (cd docker-postgres && docker-compose down)
    (cd docker-mysql && docker-compose down)
    (cd docker-mongo && docker-compose down)
    (cd docker-db2 && docker-compose down)

    print_success "All database containers removed (data preserved)"
}

# Function to remove all databases and volumes
clean_all() {
    print_error "WARNING: This will delete ALL database data!"
    read -p "Are you sure? (yes/no): " confirm

    if [ "$confirm" != "yes" ]; then
        print_status "Cancelled."
        exit 0
    fi

    print_status "Removing all databases and volumes..."

    docker-compose down -v
    (cd docker-postgres && docker-compose down -v)
    (cd docker-mysql && docker-compose down -v)
    (cd docker-mongo && docker-compose down -v)
    (cd docker-db2 && docker-compose down -v)

    print_success "All databases and data removed!"
}

# Function to show logs
logs() {
    local db=$1
    case $db in
        oracle)
            docker-compose logs -f oracle
            ;;
        postgres|postgresql)
            (cd docker-postgres && docker-compose logs -f postgres)
            ;;
        mysql)
            (cd docker-mysql && docker-compose logs -f mysql)
            ;;
        mongo|mongodb)
            (cd docker-mongo && docker-compose logs -f mongodb)
            ;;
        db2)
            (cd docker-db2 && docker-compose logs -f db2)
            ;;
        *)
            print_error "Unknown database: $db"
            print_status "Available: oracle, postgres, mysql, mongo, db2"
            exit 1
            ;;
    esac
}

# Function to initialize DB2
init_db2() {
    print_status "Initializing DB2 schema..."

    if ! docker ps --filter "name=lawgrid-db2-db" --format "{{.Names}}" | grep -q "lawgrid-db2-db"; then
        print_error "DB2 container is not running. Start it first with: $0 start db2"
        exit 1
    fi

    docker exec lawgrid-db2-db su - db2inst1 -c "db2 -tvf /var/custom/01-create-schema.sql"
    print_success "DB2 schema initialized!"
}

# Function to start individual database
start_db() {
    local db=$1
    case $db in
        oracle)
            print_status "Starting Oracle..."
            docker-compose up -d
            ;;
        postgres|postgresql)
            print_status "Starting PostgreSQL..."
            (cd docker-postgres && docker-compose up -d)
            ;;
        mysql)
            print_status "Starting MySQL..."
            (cd docker-mysql && docker-compose up -d)
            ;;
        mongo|mongodb)
            print_status "Starting MongoDB..."
            (cd docker-mongo && docker-compose up -d)
            ;;
        db2)
            print_status "Starting DB2..."
            (cd docker-db2 && docker-compose up -d)
            print_warning "Remember to initialize DB2 schema: $0 init-db2"
            ;;
        *)
            print_error "Unknown database: $db"
            print_status "Available: oracle, postgres, mysql, mongo, db2"
            exit 1
            ;;
    esac
    print_success "$db started!"
}

# Function to stop individual database
stop_db() {
    local db=$1
    case $db in
        oracle)
            docker-compose stop
            ;;
        postgres|postgresql)
            (cd docker-postgres && docker-compose stop)
            ;;
        mysql)
            (cd docker-mysql && docker-compose stop)
            ;;
        mongo|mongodb)
            (cd docker-mongo && docker-compose stop)
            ;;
        db2)
            (cd docker-db2 && docker-compose stop)
            ;;
        *)
            print_error "Unknown database: $db"
            print_status "Available: oracle, postgres, mysql, mongo, db2"
            exit 1
            ;;
    esac
    print_success "$db stopped!"
}

# Function to show usage
usage() {
    cat << EOF
LAWgrid Database Management Script

Usage: $0 <command> [options]

Commands:
  start-all              Start all databases (Oracle, PostgreSQL, MySQL, MongoDB, DB2)
  stop-all               Stop all databases
  restart-all            Restart all databases
  status                 Show status of all databases
  down-all               Remove all containers (keep data)
  clean-all              Remove all containers and data (WARNING: deletes data!)

  start <db>             Start specific database (oracle, postgres, mysql, mongo, db2)
  stop <db>              Stop specific database
  logs <db>              View logs for specific database

  init-db2               Initialize DB2 schema (run after first start)

Examples:
  $0 start-all           # Start all databases
  $0 status              # Check status
  $0 start postgres      # Start only PostgreSQL
  $0 start mongo         # Start only MongoDB
  $0 logs mysql          # View MySQL logs
  $0 init-db2            # Initialize DB2 after first start
  $0 stop-all            # Stop all databases
  $0 clean-all           # Remove everything (with confirmation)

EOF
}

# Main script logic
check_docker

case "${1:-}" in
    start-all)
        start_all
        ;;
    stop-all)
        stop_all
        ;;
    restart-all)
        restart_all
        ;;
    status)
        status_all
        ;;
    down-all)
        down_all
        ;;
    clean-all)
        clean_all
        ;;
    start)
        if [ -z "${2:-}" ]; then
            print_error "Please specify a database: oracle, postgres, mysql, db2"
            exit 1
        fi
        start_db "$2"
        ;;
    stop)
        if [ -z "${2:-}" ]; then
            print_error "Please specify a database: oracle, postgres, mysql, db2"
            exit 1
        fi
        stop_db "$2"
        ;;
    logs)
        if [ -z "${2:-}" ]; then
            print_error "Please specify a database: oracle, postgres, mysql, db2"
            exit 1
        fi
        logs "$2"
        ;;
    init-db2)
        init_db2
        ;;
    help|--help|-h)
        usage
        ;;
    *)
        print_error "Unknown command: ${1:-}"
        echo ""
        usage
        exit 1
        ;;
esac
