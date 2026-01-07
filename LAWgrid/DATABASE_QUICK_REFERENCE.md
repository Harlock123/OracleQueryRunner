# Database Quick Reference Card

Quick reference for all LAWgrid test databases. For detailed documentation, see [DOCKER_DATABASES_README.md](DOCKER_DATABASES_README.md).

## Quick Start

```bash
# Easy way - Use the management script
./manage-databases.sh start-all
./manage-databases.sh status
./manage-databases.sh init-db2  # Required for DB2 only

# Manual way - Start individual databases
docker-compose up -d                              # Oracle
cd docker-postgres && docker-compose up -d        # PostgreSQL
cd docker-mysql && docker-compose up -d           # MySQL
cd docker-mongo && docker-compose up -d           # MongoDB
cd docker-db2 && docker-compose up -d             # DB2
```

## Connection Strings

### Oracle
```csharp
"User Id=testuser;Password=TestPassword123;Data Source=localhost:1521/XEPDB1;"
```

### PostgreSQL
```csharp
"Host=localhost;Port=5432;Database=lawgrid_test;Username=testuser;Password=TestPassword123;"
```

### MySQL
```csharp
"Server=localhost;Port=3306;Database=lawgrid_test;Uid=testuser;Pwd=TestPassword123;"
```

### MongoDB (NoSQL)
```csharp
"mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test"
```

### DB2
```csharp
"Server=localhost:50000;Database=lawgrid;UID=db2inst1;PWD=TestPassword123;"
```

## LAWgrid Method Calls

### Oracle
```csharp
await grid.PopulateFromOracleQuery(connectionString, "SELECT * FROM EMPLOYEES");
await grid.PopulateFromOracleQueryAsync(connectionString, "SELECT * FROM EMPLOYEES");
grid.PopulateFromOracleQuerySync(connectionString, "SELECT * FROM EMPLOYEES");
```

### PostgreSQL
```csharp
await grid.PopulateFromPostgresQuery(connectionString, "SELECT * FROM employees");
await grid.PopulateFromPostgresQueryAsync(connectionString, "SELECT * FROM employees");
grid.PopulateFromPostgresQuerySync(connectionString, "SELECT * FROM employees");
```

### MySQL
```csharp
await grid.PopulateFromMySqlQuery(connectionString, "SELECT * FROM employees");
await grid.PopulateFromMySqlQueryAsync(connectionString, "SELECT * FROM employees");
grid.PopulateFromMySqlQuerySync(connectionString, "SELECT * FROM employees");
```

### MongoDB (NoSQL - Different Signature!)
```csharp
// MongoDB uses: (connectionString, databaseName, collectionName, filter)
await grid.PopulateFromMongoQuery(connectionString, "lawgrid_test", "employees", "{}");
await grid.PopulateFromMongoQueryAsync(connectionString, "lawgrid_test", "employees", "{}");
grid.PopulateFromMongoQuerySync(connectionString, "lawgrid_test", "employees", "{}");

// MongoDB filter examples:
await grid.PopulateFromMongoQuery(connectionString, "lawgrid_test", "employees", "{}"); // All
await grid.PopulateFromMongoQuery(connectionString, "lawgrid_test", "employees", "{\"department_id\": 60}"); // Filtered
await grid.PopulateFromMongoQuery(connectionString, "lawgrid_test", "employees", "{\"salary\": {\"$gt\": 10000}}"); // Comparison
```

### DB2
```csharp
await grid.PopulateFromDb2Query(connectionString, "SELECT * FROM employees");
await grid.PopulateFromDb2QueryAsync(connectionString, "SELECT * FROM employees");
grid.PopulateFromDb2QuerySync(connectionString, "SELECT * FROM employees");
```

## MongoDB Dynamic Schema Handling

**Special Feature:** MongoDB documents can have different fields. LAWgrid automatically:
1. Collects all unique field names across all documents
2. Creates a grid with ALL fields as columns
3. Fills empty cells for missing fields

Example:
```csharp
// Querying products collection with highly varied schemas
var result = await grid.PopulateFromMongoQueryAsync(
    connectionString,
    "lawgrid_test",
    "products",  // Electronics, Furniture, Books, Software - all different schemas!
    "{}"
);
// Result: Grid shows ALL fields from ALL product types!
// Electronics fields: specifications, warranty_years
// Furniture fields: material, dimensions, assembly_required
// Books fields: author, isbn, publisher
// Software fields: is_subscription, platforms
```

## Basic Test Queries

### SQL Databases (Oracle, PostgreSQL, MySQL, DB2)
All have tables: `employees`, `departments`, `products`

```sql
-- Oracle (uppercase)
SELECT * FROM EMPLOYEES ORDER BY EMPLOYEE_ID;

-- PostgreSQL, MySQL, DB2 (lowercase)
SELECT * FROM employees ORDER BY employee_id;

-- Join example
SELECT e.first_name, e.last_name, d.department_name
FROM employees e
LEFT JOIN departments d ON e.department_id = d.department_id;
```

### MongoDB (NoSQL)
Collections: `employees`, `departments`, `products`, `orders`

```csharp
// All employees
grid.PopulateFromMongoQuery(conn, "lawgrid_test", "employees", "{}");

// IT department
grid.PopulateFromMongoQuery(conn, "lawgrid_test", "employees", "{\"department_id\": 60}");

// High earners
grid.PopulateFromMongoQuery(conn, "lawgrid_test", "employees", "{\"salary\": {\"$gt\": 10000}}");

// Employees with skills field (only some have it)
grid.PopulateFromMongoQuery(conn, "lawgrid_test", "employees", "{\"skills\": {\"$exists\": true}}");
```

## Management Script Commands

```bash
# Start/Stop
./manage-databases.sh start-all
./manage-databases.sh stop-all
./manage-databases.sh restart-all

# Individual databases
./manage-databases.sh start postgres
./manage-databases.sh start mongo      # Start MongoDB
./manage-databases.sh stop mysql
./manage-databases.sh logs oracle

# Status and maintenance
./manage-databases.sh status
./manage-databases.sh down-all      # Remove containers, keep data
./manage-databases.sh clean-all     # Remove everything (with confirmation)

# DB2 specific
./manage-databases.sh init-db2      # Required after first start
```

## Troubleshooting Commands

### Check Status
```bash
docker ps --filter "name=lawgrid"
./manage-databases.sh status
```

### View Logs
```bash
./manage-databases.sh logs oracle
./manage-databases.sh logs postgres
./manage-databases.sh logs mysql
./manage-databases.sh logs mongo
./manage-databases.sh logs db2
```

### Connect via CLI
```bash
# Oracle
docker exec -it lawgrid-oracle-db sqlplus testuser/TestPassword123@XEPDB1

# PostgreSQL
docker exec -it lawgrid-postgres-db psql -U testuser -d lawgrid_test

# MySQL
docker exec -it lawgrid-mysql-db mysql -u testuser -pTestPassword123 lawgrid_test

# MongoDB
docker exec -it lawgrid-mongodb mongosh -u testuser -p TestPassword123 --authenticationDatabase lawgrid_test lawgrid_test

# DB2
docker exec -it lawgrid-db2-db bash
su - db2inst1
db2 connect to lawgrid
```

### Reset Database
```bash
# Stop, remove all data, and restart
docker-compose down -v && docker-compose up -d  # Oracle
cd docker-postgres && docker-compose down -v && docker-compose up -d  # PostgreSQL
cd docker-mysql && docker-compose down -v && docker-compose up -d  # MySQL
cd docker-mongo && docker-compose down -v && docker-compose up -d  # MongoDB
cd docker-db2 && docker-compose down -v && docker-compose up -d  # DB2
```

## Port Reference

| Database | Port | Type |
|----------|------|------|
| Oracle | 1521 | SQL |
| PostgreSQL | 5432 | SQL |
| MySQL | 3306 | SQL |
| MongoDB | 27017 | NoSQL |
| DB2 | 50000 | SQL |

## Common Issues

### Port Already in Use
```bash
# Check what's using the port
lsof -i :1521   # Oracle
lsof -i :5432   # PostgreSQL
lsof -i :3306   # MySQL
lsof -i :27017  # MongoDB
lsof -i :50000  # DB2
```

### Container Won't Start
```bash
# View logs
./manage-databases.sh logs <database>

# Reset completely
./manage-databases.sh clean-all
./manage-databases.sh start-all
```

### DB2 "Table Not Found"
```bash
# Initialize schema (required for DB2)
./manage-databases.sh init-db2
```

### MongoDB "Too Many Columns"
This is normal! MongoDB documents have varied schemas. Use filters to narrow results:
```csharp
// Instead of all products (many different schemas)
grid.PopulateFromMongoQuery(conn, "lawgrid_test", "products", "{}");

// Filter by category
grid.PopulateFromMongoQuery(conn, "lawgrid_test", "products", "{\"category\": \"Electronics\"}");
```

### Connection Timeout
```bash
# Wait for healthy status
./manage-databases.sh status

# Check if container is running
docker ps | grep lawgrid
```

## Resource Usage

| Database | RAM | Disk | Startup | Type |
|----------|-----|------|---------|------|
| Oracle | 2GB | 2.5GB | 2-3 min | SQL |
| PostgreSQL | 512MB | 80MB | 10-15 sec | SQL |
| MySQL | 1GB | 500MB | 30-45 sec | SQL |
| MongoDB | 512MB | 600MB | 15-20 sec | NoSQL |
| DB2 | 4GB | 5GB | 3-5 min | SQL |

**Running all together:** ~9GB RAM, ~11GB disk

## SQL vs NoSQL Comparison

| Feature | SQL Databases | MongoDB (NoSQL) |
|---------|--------------|-----------------|
| Schema | Fixed | Flexible/Dynamic |
| Query | SQL strings | JSON filters |
| Empty Fields | NULL | Field doesn't exist |
| LAWgrid Handling | Columns predefined | **Dynamic columns** |
| Joins | Built-in | Embedded docs or $lookup |
| Example | `SELECT * FROM employees` | `"employees"` collection, `"{}"` filter |

## Documentation Links

- [Complete Overview](DOCKER_DATABASES_README.md)
- [Oracle Setup](DOCKER_ORACLE_SETUP.md)
- [PostgreSQL Setup](docker-postgres/POSTGRES_SETUP.md)
- [MySQL Setup](docker-mysql/MYSQL_SETUP.md)
- [MongoDB Setup](docker-mongo/MONGODB_SETUP.md) 🌟 Dynamic Schemas!
- [DB2 Setup](docker-db2/DB2_SETUP.md)

## Test Query Files

Each database includes comprehensive test queries:
- SQL databases: `init-scripts/01-create-schema.sql` and `init-scripts/test-queries.sql`
- MongoDB: `init-scripts/01-create-collections.js` and `init-scripts/test-queries.js`

## Quick Testing Workflow

### SQL Databases
1. Start database: `./manage-databases.sh start postgres`
2. Wait for healthy: `./manage-databases.sh status`
3. Test connection with LAWgrid
4. Run SQL queries from `init-scripts/test-queries.sql`
5. Stop when done: `./manage-databases.sh stop postgres`

### MongoDB (NoSQL)
1. Start database: `./manage-databases.sh start mongo`
2. Wait for healthy: `./manage-databases.sh status`
3. Test connection with LAWgrid using JSON filters
4. Test varied schemas: query `products` or `employees` collection
5. See dynamic field handling in action!
6. Stop when done: `./manage-databases.sh stop mongo`

## MongoDB Filter Cheat Sheet

```csharp
// Basic
"{}"                                          // All documents
"{\"field\": \"value\"}"                      // Exact match
"{\"number\": 100}"                           // Number match

// Comparison
"{\"salary\": {\"$gt\": 10000}}"              // Greater than
"{\"salary\": {\"$gte\": 10000}}"             // Greater or equal
"{\"salary\": {\"$lt\": 5000}}"               // Less than
"{\"salary\": {\"$ne\": 5000}}"               // Not equal

// Field existence
"{\"skills\": {\"$exists\": true}}"           // Has field
"{\"commission_pct\": {\"$exists\": false}}"  // Doesn't have field

// Multiple conditions (AND)
"{\"department_id\": 60, \"salary\": {\"$gt\": 5000}}"

// OR conditions
"{\"$or\": [{\"department_id\": 60}, {\"department_id\": 40}]}"

// Nested fields
"{\"specifications.processor\": \"Intel i7\"}"
```
