# Oracle Database Docker Environment for LAWgrid Testing

This Docker environment provides an Oracle Database XE 21c instance for testing the LAWgrid Oracle population methods.

## Prerequisites

- Docker installed and running
- Docker Compose installed
- At least 2GB of free RAM for the Oracle container
- Accept Oracle Database license terms

## Quick Start

### 1. Start the Oracle Database

```bash
docker-compose up -d
```

The first time you run this, Docker will:
- Pull the Oracle Database XE image (~2.5GB)
- Start the container
- Run the initialization scripts to create sample data

**Note:** The initial startup may take 2-3 minutes. The database is ready when the healthcheck passes.

### 2. Check Database Status

```bash
docker-compose ps
```

Wait until the status shows "healthy".

### 3. Monitor Startup Logs (optional)

```bash
docker-compose logs -f oracle
```

Press `Ctrl+C` to stop following logs.

## Database Connection Details

| Parameter | Value |
|-----------|-------|
| **Host** | localhost |
| **Port** | 1521 |
| **Service Name** | XEPDB1 |
| **Username** | testuser |
| **Password** | TestPassword123 |
| **SYS Password** | OraclePassword123 |

### Connection String Examples

**Standard format:**
```
User Id=testuser;Password=TestPassword123;Data Source=localhost:1521/XEPDB1;
```

**TNS format:**
```
User Id=testuser;Password=TestPassword123;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));
```

**EZ Connect format:**
```
User Id=testuser;Password=TestPassword123;Data Source=localhost:1521/XEPDB1;
```

## Sample Data

The initialization script creates three tables with sample data:

### 1. EMPLOYEES Table
- 13 sample employee records
- Columns: EMPLOYEE_ID, FIRST_NAME, LAST_NAME, EMAIL, PHONE_NUMBER, HIRE_DATE, JOB_ID, SALARY, COMMISSION_PCT, MANAGER_ID, DEPARTMENT_ID

### 2. DEPARTMENTS Table
- 6 department records
- Columns: DEPARTMENT_ID, DEPARTMENT_NAME, MANAGER_ID, LOCATION_ID

### 3. PRODUCTS Table
- 10 product records
- Columns: PRODUCT_ID, PRODUCT_NAME, CATEGORY, PRICE, STOCK_QUANTITY, CREATED_DATE

## Testing LAWgrid Oracle Populators

### Example Queries for Testing

**Get all employees:**
```sql
SELECT * FROM EMPLOYEES ORDER BY EMPLOYEE_ID
```

**Get employees with department info:**
```sql
SELECT
    e.EMPLOYEE_ID,
    e.FIRST_NAME,
    e.LAST_NAME,
    e.EMAIL,
    e.SALARY,
    d.DEPARTMENT_NAME
FROM EMPLOYEES e
LEFT JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID
ORDER BY e.EMPLOYEE_ID
```

**Get all products:**
```sql
SELECT * FROM PRODUCTS ORDER BY PRODUCT_ID
```

**Get products by category:**
```sql
SELECT
    PRODUCT_NAME,
    CATEGORY,
    PRICE,
    STOCK_QUANTITY
FROM PRODUCTS
WHERE CATEGORY = 'Electronics'
ORDER BY PRICE DESC
```

**Get IT department employees:**
```sql
SELECT
    FIRST_NAME,
    LAST_NAME,
    EMAIL,
    SALARY
FROM EMPLOYEES
WHERE DEPARTMENT_ID = 60
ORDER BY SALARY DESC
```

### C# Code Example

```csharp
using LAWgrid;

// Create your LAWgrid instance
var grid = new LAWgrid();

// Connection string
string connectionString = "User Id=testuser;Password=TestPassword123;Data Source=localhost:1521/XEPDB1;";

// Test query
string query = "SELECT * FROM EMPLOYEES ORDER BY EMPLOYEE_ID";

// Async method
bool success = await grid.PopulateFromOracleQuery(connectionString, query);

// Or synchronous method
bool success = grid.PopulateFromOracleQuerySync(connectionString, query);

// Or detailed result method
var result = await grid.PopulateFromOracleQueryAsync(connectionString, query);
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
docker-compose logs oracle
```

### Connect to Oracle SQL*Plus
```bash
docker exec -it lawgrid-oracle-db sqlplus testuser/TestPassword123@XEPDB1
```

Or as SYS:
```bash
docker exec -it lawgrid-oracle-db sqlplus sys/OraclePassword123@XEPDB1 as sysdba
```

## Customizing the Setup

### Adding Your Own Data

Create additional SQL files in the `init-scripts/` directory. They will be executed in alphabetical order during container initialization.

Example: `init-scripts/02-custom-data.sql`

```sql
ALTER SESSION SET CONTAINER = XEPDB1;

INSERT INTO testuser.PRODUCTS (PRODUCT_ID, PRODUCT_NAME, CATEGORY, PRICE, STOCK_QUANTITY)
VALUES (2001, 'Custom Product', 'Custom', 99.99, 100);

COMMIT;
```

**Note:** Init scripts only run during first container creation. To re-run:
```bash
docker-compose down -v
docker-compose up -d
```

### Changing Passwords

Edit `docker-compose.yml` and change the `ORACLE_PWD` environment variable, then:
```bash
docker-compose down -v
docker-compose up -d
```

## Ports

- **1521:** Oracle Database listener (TNS)
- **5500:** Oracle Enterprise Manager Express (Web UI)
  - Access at: https://localhost:5500/em
  - Login as: `SYSTEM` / `OraclePassword123`

## Troubleshooting

### Container won't start
- Ensure you have at least 2GB of free RAM
- Check Docker logs: `docker-compose logs oracle`
- Try: `docker-compose down -v` then `docker-compose up -d`

### Connection refused
- Wait for healthcheck to pass: `docker-compose ps`
- Check container is running: `docker ps`
- Verify port 1521 is not in use by another application

### "No such table" errors
- Ensure you're connecting to XEPDB1 (not XE)
- Check if init scripts ran: `docker-compose logs oracle | grep "initialization completed"`
- Verify table ownership: tables are owned by `testuser`

### Slow performance
- Increase shared memory: edit `shm_size` in `docker-compose.yml`
- Allocate more RAM to Docker Desktop if on Windows/Mac

## License

Oracle Database XE is free to use for development and deployment. By using this Docker image, you accept Oracle's license terms.

## Resources

- [Oracle Database XE Documentation](https://docs.oracle.com/en/database/oracle/oracle-database/21/xeinl/)
- [Oracle.ManagedDataAccess Documentation](https://www.oracle.com/database/technologies/appdev/dotnet/odp.html)
- [LAWgrid Repository](https://github.com/your-repo/LAWgrid)
