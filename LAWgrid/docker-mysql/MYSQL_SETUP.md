# MySQL Database Docker Environment for LAWgrid Testing

This Docker environment provides a MySQL 8.0 instance for testing the LAWgrid MySQL population methods.

## Prerequisites

- Docker installed and running
- Docker Compose installed
- At least 1GB of free RAM

## Quick Start

### 1. Start the MySQL Database

```bash
cd docker-mysql
docker-compose up -d
```

The first time you run this, Docker will:
- Pull the MySQL 8.0 image (~500MB)
- Start the container
- Run the initialization scripts to create sample data

**Note:** The initial startup takes ~30-45 seconds.

### 2. Check Database Status

```bash
docker-compose ps
```

Wait until the status shows "healthy".

### 3. Monitor Startup Logs (optional)

```bash
docker-compose logs -f mysql
```

Press `Ctrl+C` to stop following logs.

## Database Connection Details

| Parameter | Value |
|-----------|-------|
| **Host** | localhost |
| **Port** | 3306 |
| **Database** | lawgrid_test |
| **Username** | testuser |
| **Password** | TestPassword123 |
| **Root Password** | RootPassword123 |

### Connection String Examples

**Standard format:**
```
Server=localhost;Port=3306;Database=lawgrid_test;Uid=testuser;Pwd=TestPassword123;
```

**Alternative format:**
```
server=localhost;port=3306;database=lawgrid_test;user=testuser;password=TestPassword123;
```

**With SSL disabled (for local testing):**
```
Server=localhost;Port=3306;Database=lawgrid_test;Uid=testuser;Pwd=TestPassword123;SslMode=None;
```

## Sample Data

The initialization script creates three tables with sample data:

### 1. employees Table
- 13 sample employee records
- Columns: employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id

### 2. departments Table
- 6 department records
- Columns: department_id, department_name, manager_id, location_id

### 3. products Table
- 10 product records
- Columns: product_id, product_name, category, price, stock_quantity, created_date

## Testing LAWgrid MySQL Populators

### Example Queries for Testing

**Get all employees:**
```sql
SELECT * FROM employees ORDER BY employee_id
```

**Get employees with department info:**
```sql
SELECT
    e.employee_id,
    CONCAT(e.first_name, ' ', e.last_name) AS full_name,
    e.email,
    e.salary,
    d.department_name
FROM employees e
LEFT JOIN departments d ON e.department_id = d.department_id
ORDER BY e.employee_id
```

**Get all products:**
```sql
SELECT * FROM products ORDER BY product_id
```

**MySQL-specific JSON query:**
```sql
SELECT
    d.department_name,
    JSON_ARRAYAGG(
        JSON_OBJECT(
            'name', CONCAT(e.first_name, ' ', e.last_name),
            'salary', e.salary
        )
    ) AS employees
FROM departments d
LEFT JOIN employees e ON d.department_id = d.department_id
GROUP BY d.department_name
```

### C# Code Example

```csharp
using LAWgrid;

// Create your LAWgrid instance
var grid = new LAWgrid();

// Connection string
string connectionString = "Server=localhost;Port=3306;Database=lawgrid_test;Uid=testuser;Pwd=TestPassword123;";

// Test query
string query = "SELECT * FROM employees ORDER BY employee_id";

// Async method
bool success = await grid.PopulateFromMySqlQuery(connectionString, query);

// Or synchronous method
bool success = grid.PopulateFromMySqlQuerySync(connectionString, query);

// Or detailed result method
var result = await grid.PopulateFromMySqlQueryAsync(connectionString, query);
if (result.Success)
{
    Console.WriteLine($"Successfully loaded {result.RowCount} rows");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

## Managing the Database

### Stop the database
```bash
docker-compose stop
```

### Start the database (after stopping)
```bash
docker-compose start
```

### Restart the database
```bash
docker-compose restart
```

### Stop and remove the database (keeps data volume)
```bash
docker-compose down
```

### Stop and remove everything including data
```bash
docker-compose down -v
```

### View logs
```bash
docker-compose logs mysql
```

### Connect to MySQL CLI
```bash
docker exec -it lawgrid-mysql-db mysql -u testuser -pTestPassword123 lawgrid_test
```

Or as root:
```bash
docker exec -it lawgrid-mysql-db mysql -u root -pRootPassword123
```

Once connected, useful MySQL commands:
- `SHOW TABLES;` - List all tables
- `DESCRIBE employees;` - Describe employees table
- `SHOW DATABASES;` - List all databases
- `exit` - Quit MySQL

## Customizing the Setup

### Adding Your Own Data

Create additional SQL files in the `init-scripts/` directory. They will be executed in alphabetical order during container initialization.

Example: `init-scripts/02-custom-data.sql`

```sql
USE lawgrid_test;

INSERT INTO products (product_name, category, price, stock_quantity)
VALUES ('Custom Product', 'Custom', 99.99, 100);
```

**Note:** Init scripts only run during first container creation. To re-run:
```bash
docker-compose down -v
docker-compose up -d
```

### Changing Passwords

Edit `docker-compose.yml` and change:
- `MYSQL_PASSWORD` for testuser
- `MYSQL_ROOT_PASSWORD` for root user

Then:
```bash
docker-compose down -v
docker-compose up -d
```

## MySQL-Specific Features

MySQL 8.0 offers many features you can test with LAWgrid:

### JSON Support (MySQL 5.7+)
```sql
SELECT JSON_OBJECT('name', first_name, 'salary', salary)
FROM employees
LIMIT 1;
```

### Window Functions (MySQL 8.0+)
```sql
SELECT
    first_name,
    last_name,
    salary,
    RANK() OVER (ORDER BY salary DESC) AS salary_rank
FROM employees;
```

### Common Table Expressions (MySQL 8.0+)
```sql
WITH high_earners AS (
    SELECT * FROM employees WHERE salary > 10000
)
SELECT COUNT(*) FROM high_earners;
```

### Full-Text Search
First create index (if not exists):
```sql
ALTER TABLE products ADD FULLTEXT(product_name);
```

Then search:
```sql
SELECT product_name, category, price
FROM products
WHERE MATCH(product_name) AGAINST('laptop');
```

## Character Set and Collation

The database is configured with:
- Character Set: `utf8mb4` (full Unicode support, including emojis)
- Collation: `utf8mb4_unicode_ci` (case-insensitive)

To check:
```sql
SHOW VARIABLES LIKE 'character_set%';
SHOW VARIABLES LIKE 'collation%';
```

## Troubleshooting

### Container won't start
- Ensure port 3306 is not in use: `lsof -i :3306` (on Linux/Mac) or `netstat -an | findstr 3306` (on Windows)
- Check Docker logs: `docker-compose logs mysql`
- Try: `docker-compose down -v` then `docker-compose up -d`

### Connection refused or authentication errors
- Wait for healthcheck to pass: `docker-compose ps`
- Check container is running: `docker ps`
- Verify connection string has correct username/password
- For MySQL 8.0, ensure your client supports `caching_sha2_password` or use `mysql_native_password`

### "No such table" errors
- Ensure init scripts ran: `docker-compose logs mysql | grep "lawgrid_test"`
- Check you're connecting to `lawgrid_test` database
- Verify tables exist: `docker exec -it lawgrid-mysql-db mysql -u testuser -pTestPassword123 lawgrid_test -e "SHOW TABLES;"`

### Authentication plugin errors
The docker-compose.yml is configured to use `mysql_native_password` for compatibility. If you still have issues, check your MySQL connector version.

### Slow queries
- Check slow query log (if enabled)
- Use `EXPLAIN` to analyze query performance
- Ensure indexes are created (init script includes indexes)

## Performance Tips

- Use connection pooling in production applications
- Enable query caching for read-heavy workloads
- Monitor performance with `SHOW PROCESSLIST;`
- Use `EXPLAIN ANALYZE` for query optimization

## Backup and Restore

### Backup database
```bash
docker exec lawgrid-mysql-db mysqldump -u testuser -pTestPassword123 lawgrid_test > backup.sql
```

### Restore database
```bash
docker exec -i lawgrid-mysql-db mysql -u testuser -pTestPassword123 lawgrid_test < backup.sql
```

## Resources

- [MySQL 8.0 Documentation](https://dev.mysql.com/doc/refman/8.0/en/)
- [MySQL Connector/NET Documentation](https://dev.mysql.com/doc/connector-net/en/)
- [LAWgrid Repository](https://github.com/your-repo/LAWgrid)
