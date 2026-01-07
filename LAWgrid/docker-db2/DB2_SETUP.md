# DB2 Database Docker Environment for LAWgrid Testing

This Docker environment provides an IBM DB2 Community Edition 11.5 instance for testing the LAWgrid DB2 population methods.

## Prerequisites

- Docker installed and running
- Docker Compose installed
- At least 4GB of free RAM (DB2 is resource-intensive)
- At least 5GB of free disk space
- Docker configured to allow privileged containers

**IMPORTANT:** DB2 requires more resources than other databases. Ensure your Docker environment has adequate resources allocated.

## Quick Start

### 1. Start the DB2 Database

```bash
cd docker-db2
docker-compose up -d
```

The first time you run this, Docker will:
- Pull the DB2 Community Edition image (~2GB)
- Start the container
- Initialize the DB2 instance (this is slow!)
- Create the database

**Note:** The initial startup takes **3-5 minutes**. DB2 initialization is resource-intensive. Be patient!

### 2. Check Database Status

```bash
docker-compose ps
```

Wait until the status shows "healthy". This may take several minutes.

### 3. Monitor Startup Logs

```bash
docker-compose logs -f db2
```

Look for messages indicating DB2 is ready:
- `(*) Setup has completed.`
- `(*) User chose to create DB: lawgrid`

Press `Ctrl+C` to stop following logs.

### 4. Manually Initialize Schema (Required)

DB2's Docker image doesn't automatically run init scripts, so you need to run them manually after the container is healthy:

```bash
# Wait for container to be healthy
docker-compose ps

# Copy and run the init script
docker exec lawgrid-db2-db su - db2inst1 -c "db2 -tvf /var/custom/01-create-schema.sql"
```

You should see output confirming tables were created and data was inserted.

## Database Connection Details

| Parameter | Value |
|-----------|-------|
| **Host** | localhost |
| **Port** | 50000 |
| **Database** | lawgrid |
| **Username** | db2inst1 |
| **Password** | TestPassword123 |

### Connection String Examples

**Standard format:**
```
Server=localhost:50000;Database=lawgrid;UID=db2inst1;PWD=TestPassword123;
```

**Alternative format:**
```
Database=lawgrid;Server=localhost:50000;UserID=db2inst1;Password=TestPassword123;
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

## Testing LAWgrid DB2 Populators

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

**DB2-specific FETCH FIRST:**
```sql
SELECT
    product_name,
    category,
    price
FROM products
ORDER BY price DESC
FETCH FIRST 5 ROWS ONLY
```

### C# Code Example

```csharp
using LAWgrid;

// Create your LAWgrid instance
var grid = new LAWgrid();

// Connection string
string connectionString = "Server=localhost:50000;Database=lawgrid;UID=db2inst1;PWD=TestPassword123;";

// Test query
string query = "SELECT * FROM employees ORDER BY employee_id";

// Async method
bool success = await grid.PopulateFromDb2Query(connectionString, query);

// Or synchronous method
bool success = grid.PopulateFromDb2QuerySync(connectionString, query);

// Or detailed result method
var result = await grid.PopulateFromDb2QueryAsync(connectionString, query);
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

**Note:** Stopping takes ~30 seconds as DB2 shuts down gracefully.

### Start the database (after stopping)
```bash
docker-compose start
```

### Restart the database
```bash
docker-compose restart
```

**Warning:** Restart takes several minutes.

### Stop and remove the database (keeps data volume)
```bash
docker-compose down
```

### Stop and remove everything including data
```bash
docker-compose down -v
```

**Warning:** This will delete all data!

### View logs
```bash
docker-compose logs db2
```

### Connect to DB2 CLI
```bash
docker exec -it lawgrid-db2-db bash
su - db2inst1
db2 connect to lawgrid
```

Once connected, useful DB2 commands:
- `LIST TABLES` - List all tables
- `DESCRIBE TABLE employees` - Describe employees table
- `LIST APPLICATIONS` - Show active connections
- `QUIT` - Exit DB2 CLI
- `exit` - Exit shell

## Customizing the Setup

### Adding Your Own Data

Create additional SQL files in the `init-scripts/` directory, then run them manually:

Example: `init-scripts/02-custom-data.sql`

```sql
CONNECT TO lawgrid;

INSERT INTO products (product_id, product_name, category, price, stock_quantity)
VALUES (2001, 'Custom Product', 'Custom', 99.99, 100);

COMMIT;
CONNECT RESET;
```

Then execute:
```bash
docker exec lawgrid-db2-db su - db2inst1 -c "db2 -tvf /var/custom/02-custom-data.sql"
```

### Changing Passwords

Edit `docker-compose.yml` and change `DB2INST1_PASSWORD`, then:
```bash
docker-compose down -v
docker-compose up -d
# Wait for healthy status
docker exec lawgrid-db2-db su - db2inst1 -c "db2 -tvf /var/custom/01-create-schema.sql"
```

## DB2-Specific Features

DB2 offers many powerful features you can test with LAWgrid:

### FETCH FIRST (instead of LIMIT)
```sql
SELECT * FROM employees
ORDER BY salary DESC
FETCH FIRST 10 ROWS ONLY;
```

### LISTAGG (String Aggregation)
```sql
SELECT
    d.department_name,
    LISTAGG(e.first_name || ' ' || e.last_name, ', ')
        WITHIN GROUP (ORDER BY e.employee_id) AS employees
FROM departments d
LEFT JOIN employees e ON d.department_id = e.department_id
GROUP BY d.department_name;
```

### WITH clause (Common Table Expressions)
```sql
WITH high_earners AS (
    SELECT * FROM employees WHERE salary > 10000
)
SELECT
    first_name || ' ' || last_name AS name,
    salary
FROM high_earners
ORDER BY salary DESC;
```

### Window Functions
```sql
SELECT
    first_name,
    last_name,
    salary,
    RANK() OVER (ORDER BY salary DESC) AS salary_rank,
    ROW_NUMBER() OVER (PARTITION BY department_id ORDER BY salary DESC) AS dept_rank
FROM employees;
```

### Using SYSIBM.SYSDUMMY1
DB2's equivalent of Oracle's DUAL table:
```sql
SELECT CURRENT TIMESTAMP FROM SYSIBM.SYSDUMMY1;
```

## Troubleshooting

### Container won't start or crashes
- **Ensure you have enough RAM:** DB2 requires at least 4GB
- Check Docker resource limits (Docker Desktop settings)
- Ensure privileged mode is enabled in docker-compose.yml
- Check logs: `docker-compose logs db2`

### Container starts but healthcheck fails
- DB2 initialization takes 3-5 minutes on first run
- Wait longer and check logs: `docker-compose logs -f db2`
- Look for "Setup has completed" message
- If it never completes, you may not have enough resources

### Connection refused
- Ensure port 50000 is not in use: `lsof -i :50000`
- Wait for healthcheck to pass: `docker-compose ps`
- Verify DB2 is running: `docker exec lawgrid-db2-db su - db2inst1 -c "db2 list applications"`

### "No such table" errors
- **Most common issue:** Init scripts must be run manually
- Run: `docker exec lawgrid-db2-db su - db2inst1 -c "db2 -tvf /var/custom/01-create-schema.sql"`
- Verify tables exist: `docker exec lawgrid-db2-db su - db2inst1 -c "db2 connect to lawgrid && db2 list tables"`

### SQL Error codes
- **SQL0204N**: Table not found - run init scripts
- **SQL1024N**: Database not found - ensure lawgrid database was created
- **SQL30082N**: Connection refused - ensure DB2 is running and port is correct
- **SQL0911N**: Deadlock - transaction rolled back, retry

### Performance is very slow
- DB2 is resource-intensive, especially in Docker
- Allocate more RAM to Docker (at least 4GB)
- DB2 Community Edition is not optimized for performance
- Consider using a native DB2 installation for production testing

### License acceptance
If you get license errors, ensure `LICENSE=accept` is set in docker-compose.yml.

## Important DB2 Considerations

1. **Resource Requirements:** DB2 uses significantly more resources than MySQL/PostgreSQL
2. **Startup Time:** First startup takes 3-5 minutes; subsequent starts are faster (~1 minute)
3. **Manual Initialization:** Init scripts must be run manually (not automatic like other databases)
4. **Case Sensitivity:** By default, unquoted identifiers are uppercase in DB2
5. **String Concatenation:** Use `||` operator, not `+`
6. **Date Functions:** Different from other databases (e.g., `CURRENT TIMESTAMP` not `NOW()`)

## Running Multiple Databases

You can run multiple database containers simultaneously for comprehensive testing:

```bash
# From project root
cd docker-postgres && docker-compose up -d && cd ..
cd docker-mysql && docker-compose up -d && cd ..
cd docker-db2 && docker-compose up -d && cd ..

# Check all running databases
docker ps
```

Each uses different ports:
- PostgreSQL: 5432
- MySQL: 3306
- DB2: 50000
- Oracle: 1521

## Backup and Restore

### Backup database
```bash
docker exec lawgrid-db2-db su - db2inst1 -c "db2 backup database lawgrid to /tmp"
docker cp lawgrid-db2-db:/tmp/LAWGRID.0.db2inst1.DBPART000.* ./
```

### Restore database
```bash
docker cp ./LAWGRID.* lawgrid-db2-db:/tmp/
docker exec lawgrid-db2-db su - db2inst1 -c "db2 restore database lawgrid from /tmp"
```

## Resources

- [DB2 Documentation](https://www.ibm.com/docs/en/db2/11.5)
- [DB2 Docker Hub](https://hub.docker.com/r/ibmcom/db2)
- [IBM Data Server Provider for .NET](https://www.ibm.com/support/pages/ibm-data-server-client-packages-version-115-mod-5-fix-pack-5)
- [LAWgrid Repository](https://github.com/your-repo/LAWgrid)

## License

DB2 Community Edition is free for development and testing. By using this Docker image, you accept IBM's license terms. The `LICENSE=accept` environment variable in docker-compose.yml indicates acceptance.
