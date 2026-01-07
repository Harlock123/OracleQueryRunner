# PostgreSQL Database Docker Environment for LAWgrid Testing

This Docker environment provides a PostgreSQL 16 instance for testing the LAWgrid PostgreSQL population methods.

## Prerequisites

- Docker installed and running
- Docker Compose installed
- At least 512MB of free RAM

## Quick Start

### 1. Start the PostgreSQL Database

```bash
cd docker-postgres
docker-compose up -d
```

The first time you run this, Docker will:
- Pull the PostgreSQL 16 Alpine image (~80MB)
- Start the container
- Run the initialization scripts to create sample data

**Note:** The initial startup takes ~10-15 seconds.

### 2. Check Database Status

```bash
docker-compose ps
```

Wait until the status shows "healthy".

### 3. Monitor Startup Logs (optional)

```bash
docker-compose logs -f postgres
```

Press `Ctrl+C` to stop following logs.

## Database Connection Details

| Parameter | Value |
|-----------|-------|
| **Host** | localhost |
| **Port** | 5432 |
| **Database** | lawgrid_test |
| **Username** | testuser |
| **Password** | TestPassword123 |

### Connection String Examples

**Standard format:**
```
Host=localhost;Port=5432;Database=lawgrid_test;Username=testuser;Password=TestPassword123;
```

**With SSL disabled (for local testing):**
```
Host=localhost;Port=5432;Database=lawgrid_test;Username=testuser;Password=TestPassword123;SSL Mode=Disable;
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

## Testing LAWgrid PostgreSQL Populators

### Example Queries for Testing

**Get all employees:**
```sql
SELECT * FROM employees ORDER BY employee_id
```

**Get employees with department info:**
```sql
SELECT
    e.employee_id,
    e.first_name || ' ' || e.last_name AS full_name,
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

**PostgreSQL-specific JSON query:**
```sql
SELECT
    d.department_name,
    json_agg(
        json_build_object(
            'name', e.first_name || ' ' || e.last_name,
            'salary', e.salary
        )
    ) AS employees
FROM departments d
LEFT JOIN employees e ON d.department_id = e.department_id
GROUP BY d.department_name
```

### C# Code Example

```csharp
using LAWgrid;

// Create your LAWgrid instance
var grid = new LAWgrid();

// Connection string
string connectionString = "Host=localhost;Port=5432;Database=lawgrid_test;Username=testuser;Password=TestPassword123;SSL Mode=Disable;";

// Test query
string query = "SELECT * FROM employees ORDER BY employee_id";

// Async method
bool success = await grid.PopulateFromPostgresQuery(connectionString, query);

// Or synchronous method
bool success = grid.PopulateFromPostgresQuerySync(connectionString, query);

// Or detailed result method
var result = await grid.PopulateFromPostgresQueryAsync(connectionString, query);
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
docker-compose logs postgres
```

### Connect to PostgreSQL CLI (psql)
```bash
docker exec -it lawgrid-postgres-db psql -U testuser -d lawgrid_test
```

Once connected, useful psql commands:
- `\dt` - List all tables
- `\d employees` - Describe employees table
- `\q` - Quit psql

## Customizing the Setup

### Adding Your Own Data

Create additional SQL files in the `init-scripts/` directory. They will be executed in alphabetical order during container initialization.

Example: `init-scripts/02-custom-data.sql`

```sql
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
- `POSTGRES_PASSWORD` environment variable
- Update connection string in your code

Then:
```bash
docker-compose down -v
docker-compose up -d
```

## PostgreSQL-Specific Features

PostgreSQL offers many advanced features you can test with LAWgrid:

### JSON Support
```sql
SELECT row_to_json(e) FROM employees e LIMIT 1;
```

### Array Support
```sql
SELECT ARRAY_AGG(first_name) FROM employees;
```

### Full-Text Search
```sql
SELECT product_name
FROM products
WHERE to_tsvector('english', product_name) @@ to_tsquery('laptop');
```

### Window Functions
```sql
SELECT
    first_name,
    last_name,
    salary,
    RANK() OVER (ORDER BY salary DESC) AS salary_rank
FROM employees;
```

## Troubleshooting

### Container won't start
- Ensure port 5432 is not in use: `lsof -i :5432` (on Linux/Mac) or `netstat -an | findstr 5432` (on Windows)
- Check Docker logs: `docker-compose logs postgres`
- Try: `docker-compose down -v` then `docker-compose up -d`

### Connection refused
- Wait for healthcheck to pass: `docker-compose ps`
- Check container is running: `docker ps`
- Verify connection string has correct parameters

### "No such table" errors
- Ensure init scripts ran: `docker-compose logs postgres | grep "initialization"`
- Check you're connecting to `lawgrid_test` database (not `postgres`)
- Verify tables exist: `docker exec -it lawgrid-postgres-db psql -U testuser -d lawgrid_test -c "\dt"`

### Permission denied errors
- PostgreSQL uses strict permissions
- Ensure you're using the `testuser` account with `lawgrid_test` database

## Performance Tips

- Use connection pooling in production applications
- Enable query logging for debugging: Add `-c log_statement=all` to command in docker-compose.yml
- Monitor slow queries: Check `pg_stat_statements` extension

## Resources

- [PostgreSQL Documentation](https://www.postgresql.org/docs/16/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [LAWgrid Repository](https://github.com/your-repo/LAWgrid)
