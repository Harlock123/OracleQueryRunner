# LAWgrid Database Docker Environments

Complete Docker environments for testing all LAWgrid database population methods with Oracle, PostgreSQL, MySQL, and DB2.

## Overview

This repository includes Docker Compose configurations for four different database systems, each with sample data and test queries specifically designed for testing LAWgrid's database population features.

| Database | Directory | Port | Status |
|----------|-----------|------|--------|
| **Oracle XE 21c** | `docker-compose.yml` (root) | 1521 | ✅ Ready |
| **PostgreSQL 16** | `docker-postgres/` | 5432 | ✅ Ready |
| **MySQL 8.0** | `docker-mysql/` | 3306 | ✅ Ready |
| **MongoDB 7.0** | `docker-mongo/` | 27017 | ✅ Ready (NoSQL) |
| **DB2 11.5** | `docker-db2/` | 50000 | ✅ Ready |

## Quick Start Guide

### Starting All Databases

You can run all databases simultaneously for comprehensive testing:

```bash
# Start Oracle (from root directory)
docker-compose up -d

# Start PostgreSQL
cd docker-postgres && docker-compose up -d && cd ..

# Start MySQL
cd docker-mysql && docker-compose up -d && cd ..

# Start MongoDB
cd docker-mongo && docker-compose up -d && cd ..

# Start DB2
cd docker-db2 && docker-compose up -d && cd ..

# Check status of all databases
docker ps
```

### Starting Individual Databases

#### Oracle Database
```bash
# From project root
docker-compose up -d
docker-compose ps  # Wait for "healthy" status
```
📖 [Full Oracle Documentation](DOCKER_ORACLE_SETUP.md)

#### PostgreSQL
```bash
cd docker-postgres
docker-compose up -d
docker-compose ps  # Wait for "healthy" status
```
📖 [Full PostgreSQL Documentation](docker-postgres/POSTGRES_SETUP.md)

#### MySQL
```bash
cd docker-mysql
docker-compose up -d
docker-compose ps  # Wait for "healthy" status
```
📖 [Full MySQL Documentation](docker-mysql/MYSQL_SETUP.md)

#### MongoDB
```bash
cd docker-mongo
docker-compose up -d
docker-compose ps  # Wait for "healthy" status
```
📖 [Full MongoDB Documentation](docker-mongo/MONGODB_SETUP.md)
🌟 **Special Feature:** Dynamic schema handling for NoSQL documents!

#### DB2
```bash
cd docker-db2
docker-compose up -d
docker-compose ps  # Wait for "healthy" status (takes 3-5 minutes)

# IMPORTANT: DB2 requires manual schema initialization
docker exec lawgrid-db2-db su - db2inst1 -c "db2 -tvf /var/custom/01-create-schema.sql"
```
📖 [Full DB2 Documentation](docker-db2/DB2_SETUP.md)

## Connection Information

All databases include the same sample data (employees, departments, products tables) for consistent testing.

### Oracle
```
Host: localhost
Port: 1521
Service: XEPDB1
Username: testuser
Password: TestPassword123

Connection String:
User Id=testuser;Password=TestPassword123;Data Source=localhost:1521/XEPDB1;
```

### PostgreSQL
```
Host: localhost
Port: 5432
Database: lawgrid_test
Username: testuser
Password: TestPassword123

Connection String:
Host=localhost;Port=5432;Database=lawgrid_test;Username=testuser;Password=TestPassword123;
```

### MySQL
```
Host: localhost
Port: 3306
Database: lawgrid_test
Username: testuser
Password: TestPassword123

Connection String:
Server=localhost;Port=3306;Database=lawgrid_test;Uid=testuser;Pwd=TestPassword123;
```

### MongoDB
```
Host: localhost
Port: 27017
Database: lawgrid_test
Username: testuser
Password: TestPassword123

Connection String:
mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test
```

### DB2
```
Host: localhost
Port: 50000
Database: lawgrid
Username: db2inst1
Password: TestPassword123

Connection String:
Server=localhost:50000;Database=lawgrid;UID=db2inst1;PWD=TestPassword123;
```

## Sample Data

Each database contains identical sample data:

### Tables

1. **employees** - 13 employee records
   - employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id

2. **departments** - 6 department records
   - department_id, department_name, manager_id, location_id

3. **products** - 10 product records
   - product_id, product_name, category, price, stock_quantity, created_date

### Sample Queries

Each database directory includes a `test-queries.sql` file with 15+ test queries ranging from simple SELECTs to complex JOINs and database-specific features.

## Testing LAWgrid Populators

### Oracle Example
```csharp
var grid = new LAWgrid();
string connectionString = "User Id=testuser;Password=TestPassword123;Data Source=localhost:1521/XEPDB1;";
string query = "SELECT * FROM EMPLOYEES ORDER BY EMPLOYEE_ID";
var result = await grid.PopulateFromOracleQueryAsync(connectionString, query);
```

### PostgreSQL Example
```csharp
var grid = new LAWgrid();
string connectionString = "Host=localhost;Port=5432;Database=lawgrid_test;Username=testuser;Password=TestPassword123;";
string query = "SELECT * FROM employees ORDER BY employee_id";
var result = await grid.PopulateFromPostgresQueryAsync(connectionString, query);
```

### MySQL Example
```csharp
var grid = new LAWgrid();
string connectionString = "Server=localhost;Port=3306;Database=lawgrid_test;Uid=testuser;Pwd=TestPassword123;";
string query = "SELECT * FROM employees ORDER BY employee_id";
var result = await grid.PopulateFromMySqlQueryAsync(connectionString, query);
```

### MongoDB Example (with Dynamic Schema Handling)
```csharp
var grid = new LAWgrid();
string connectionString = "mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test";
// Note: MongoDB uses collection name and filter instead of SQL query
var result = await grid.PopulateFromMongoQueryAsync(connectionString, "lawgrid_test", "employees", "{}");
// Get all employees - grid automatically handles varying schemas!
```

### DB2 Example
```csharp
var grid = new LAWgrid();
string connectionString = "Server=localhost:50000;Database=lawgrid;UID=db2inst1;PWD=TestPassword123;";
string query = "SELECT * FROM employees ORDER BY employee_id";
var result = await grid.PopulateFromDb2QueryAsync(connectionString, query);
```

## Resource Requirements

| Database | RAM | Disk | Startup Time | Notes |
|----------|-----|------|--------------|-------|
| Oracle | 2GB | 2.5GB | 2-3 min | First startup slow |
| PostgreSQL | 512MB | 80MB | 10-15 sec | Fastest to start |
| MySQL | 1GB | 500MB | 30-45 sec | Good balance |
| MongoDB | 512MB | 600MB | 15-20 sec | NoSQL, dynamic schemas |
| DB2 | 4GB | 5GB | 3-5 min | Most resource-intensive |

**Recommended for running all simultaneously:** 9GB RAM, 11GB disk space

## Common Operations

### Check Status of All Databases
```bash
docker ps --filter "name=lawgrid"
```

### Stop All Databases
```bash
# Stop Oracle
docker-compose stop

# Stop PostgreSQL
cd docker-postgres && docker-compose stop && cd ..

# Stop MySQL
cd docker-mysql && docker-compose stop && cd ..

# Stop MongoDB
cd docker-mongo && docker-compose stop && cd ..

# Stop DB2
cd docker-db2 && docker-compose stop && cd ..
```

### Remove All Databases (Keep Data)
```bash
docker-compose down
cd docker-postgres && docker-compose down && cd ..
cd docker-mysql && docker-compose down && cd ..
cd docker-mongo && docker-compose down && cd ..
cd docker-db2 && docker-compose down && cd ..
```

### Remove All Databases (Delete Data)
```bash
docker-compose down -v
cd docker-postgres && docker-compose down -v && cd ..
cd docker-mysql && docker-compose down -v && cd ..
cd docker-mongo && docker-compose down -v && cd ..
cd docker-db2 && docker-compose down -v && cd ..
```

### View Logs for Specific Database
```bash
# Oracle
docker-compose logs oracle

# PostgreSQL
cd docker-postgres && docker-compose logs postgres

# MySQL
cd docker-mysql && docker-compose logs mysql

# MongoDB
cd docker-mongo && docker-compose logs mongodb

# DB2
cd docker-db2 && docker-compose logs db2
```

## Testing Workflow

1. **Start the database** you want to test
2. **Wait for healthy status** (check with `docker-compose ps`)
3. **Run manual init for DB2** if testing DB2
4. **Use test queries** from `init-scripts/test-queries.sql` in each directory
5. **Test LAWgrid methods** using the connection strings above
6. **Stop the database** when done

## Database-Specific Notes

### Oracle
- Uses Oracle XE (Express Edition) - free version
- Requires accepting Oracle license terms
- Service name is `XEPDB1` (pluggable database)
- Init scripts run automatically

### PostgreSQL
- Lightweight and fast
- Excellent for development
- Full SQL standard compliance
- Best choice for quick testing

### MySQL
- Most widely used open-source database
- Good balance of features and performance
- UTF8MB4 encoding for full Unicode support
- Compatible with MariaDB clients

### MongoDB
- NoSQL document database
- **Dynamic schema handling** - documents can have different fields
- LAWgrid automatically merges all fields from all documents
- Perfect for flexible, evolving data structures
- Excellent for nested documents and arrays

### DB2
- Enterprise-grade database
- Most resource-intensive
- **Requires manual schema initialization**
- Slower startup (3-5 minutes)
- Best for testing enterprise scenarios

## Troubleshooting

### Port Already in Use
If you get port conflicts, check what's using the ports:
```bash
# Linux/Mac
lsof -i :1521  # Oracle
lsof -i :5432  # PostgreSQL
lsof -i :3306  # MySQL
lsof -i :50000 # DB2

# Windows
netstat -an | findstr 1521
netstat -an | findstr 5432
netstat -an | findstr 3306
netstat -an | findstr 50000
```

### Container Won't Start
1. Check Docker resources (RAM, disk)
2. View logs: `docker-compose logs <service-name>`
3. Remove and recreate: `docker-compose down -v && docker-compose up -d`

### Connection Refused
1. Wait for healthcheck to pass: `docker-compose ps`
2. Verify container is running: `docker ps`
3. Check connection string parameters
4. Test connection from command line first

### Init Scripts Didn't Run
- **Oracle/PostgreSQL/MySQL:** Init scripts run automatically on first startup only
- **DB2:** Init scripts MUST be run manually
- To re-run init scripts: `docker-compose down -v && docker-compose up -d`

## Advanced Usage

### Custom Data

Add your own SQL files to `init-scripts/` in any database directory:

```bash
# Example for PostgreSQL
echo "INSERT INTO products (product_name, category, price, stock_quantity) VALUES ('My Product', 'Custom', 199.99, 50);" > docker-postgres/init-scripts/02-my-data.sql

# Recreate database to run new scripts
cd docker-postgres
docker-compose down -v
docker-compose up -d
```

### Connecting from External Tools

All databases are accessible from external tools like:
- DBeaver
- DataGrip
- pgAdmin (PostgreSQL)
- MySQL Workbench
- IBM Data Studio (DB2)

Use the connection information provided above.

### Performance Testing

All databases include indexes on common foreign keys for realistic performance testing.

## Cleanup

To completely remove all databases and free up disk space:

```bash
# Stop and remove all containers and volumes
docker-compose down -v
cd docker-postgres && docker-compose down -v && cd ..
cd docker-mysql && docker-compose down -v && cd ..
cd docker-db2 && docker-compose down -v && cd ..

# Remove unused Docker resources
docker system prune -a --volumes
```

**Warning:** This deletes all data permanently!

## Support

For issues specific to:
- **LAWgrid:** Check the main repository issues
- **Docker configurations:** Check individual `*_SETUP.md` files in each directory
- **Database-specific issues:** Refer to official database documentation

## License Notes

- **Oracle Database XE:** Free for development, requires license acceptance
- **PostgreSQL:** Open source (PostgreSQL License)
- **MySQL:** Open source (GPL v2), Commercial available
- **DB2 Community Edition:** Free for development and testing, requires license acceptance

By using these Docker images, you accept the respective database licenses.

## Next Steps

1. ✅ Start with PostgreSQL (fastest and easiest)
2. ✅ Test MySQL for compatibility
3. ✅ Test Oracle for enterprise features
4. ✅ Test DB2 for maximum compatibility testing (most complex)

For detailed documentation on each database, see the respective `*_SETUP.md` files in each directory.
