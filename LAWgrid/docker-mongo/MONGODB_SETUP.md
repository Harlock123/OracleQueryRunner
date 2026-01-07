# MongoDB Database Docker Environment for LAWgrid Testing

This Docker environment provides a MongoDB 7.0 instance for testing the LAWgrid MongoDB population methods with support for **dynamic schema handling**.

## 🌟 Special Feature: Dynamic Schema Handling

Unlike relational databases, MongoDB documents in the same collection can have **different fields**. LAWgrid's MongoDB populator automatically:

1. **Collects all unique field names** across all documents
2. **Creates a unified grid** with all fields as columns
3. **Fills empty cells** for documents missing certain fields

This means you can query collections with varied schemas and get a complete view of all data!

## Prerequisites

- Docker installed and running
- Docker Compose installed
- At least 512MB of free RAM
- At least 1GB of free disk space

## Quick Start

### 1. Start the MongoDB Database

```bash
cd docker-mongo
docker-compose up -d
```

The first time you run this, Docker will:
- Pull the MongoDB 7.0 image (~600MB)
- Start the container
- Run the initialization scripts to create sample data

**Note:** The initial startup takes ~15-20 seconds.

### 2. Check Database Status

```bash
docker-compose ps
```

Wait until the status shows "healthy".

### 3. Monitor Startup Logs (optional)

```bash
docker-compose logs -f mongodb
```

Press `Ctrl+C` to stop following logs.

## Database Connection Details

| Parameter | Value |
|-----------|-------|
| **Host** | localhost |
| **Port** | 27017 |
| **Database** | lawgrid_test |
| **Username** | testuser |
| **Password** | TestPassword123 |
| **Admin Username** | admin |
| **Admin Password** | AdminPassword123 |

### Connection String Examples

**Standard format:**
```
mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test
```

**With authentication database:**
```
mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test?authSource=lawgrid_test
```

**Admin connection:**
```
mongodb://admin:AdminPassword123@localhost:27017
```

## Sample Data with Varied Schemas

The initialization script creates four collections demonstrating different schema patterns:

### 1. employees Collection (13 documents)
**Demonstrates varied schemas** - different employees have different fields:

- Standard employees: `employee_id`, `first_name`, `last_name`, `email`, `hire_date`, `salary`, `department_id`
- Some have: `commission_pct`, `manager_id`, `phone_number`
- Tech staff have: `skills` (array), `remote_work_eligible`
- Some have: `certifications`, `performance_rating`, `emergency_contact` (nested doc)
- Contract workers have: `contract_end_date`, `hourly_rate`, `employment_type`
- Part-time have: `hours_per_week`

**This collection is perfect for testing dynamic field handling!**

### 2. departments Collection (6 documents)
Mostly uniform schema with some variations:
- All have: `department_id`, `department_name`, `manager_id`, `location_id`
- Some have: `budget`, `floor`

### 3. products Collection (13 documents)
**Highly varied schemas** - different product types have different attributes:

- Electronics have: `specifications` (nested), `warranty_years`, `color_options` (array)
- Furniture have: `material`, `dimensions` (nested), `assembly_required`
- Books have: `author`, `publisher`, `isbn`, `pages`
- Software have: `is_subscription`, `platforms` (array), `cloud_storage_gb`

**Excellent for testing complex nested documents and arrays!**

### 4. orders Collection (4 documents)
Demonstrates embedded documents and arrays:
- All have: `order_id`, `customer_name`, `order_date`, `items` (array), `total_amount`
- Some have: `discount_code`, `tracking_number`, `shipping_address` (nested)
- Some have: `gift_wrap`, `gift_message`

## Testing LAWgrid MongoDB Populators

### Basic Usage Example

```csharp
using LAWgrid;

var grid = new LAWgrid();

// Connection string
string connectionString = "mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test";

// Get all employees (demonstrates dynamic field handling)
bool success = await grid.PopulateFromMongoQuery(
    connectionString,
    "lawgrid_test",
    "employees",
    "{}"  // Empty filter = get all documents
);
```

### Example Queries

**Get all employees:**
```csharp
await grid.PopulateFromMongoQuery(connectionString, "lawgrid_test", "employees", "{}");
// Result: Grid with ALL fields from ALL employees, even if only some have certain fields
```

**Filter by department:**
```csharp
await grid.PopulateFromMongoQuery(
    connectionString,
    "lawgrid_test",
    "employees",
    "{\"department_id\": 60}"  // IT department only
);
```

**Get high earners:**
```csharp
await grid.PopulateFromMongoQuery(
    connectionString,
    "lawgrid_test",
    "employees",
    "{\"salary\": {\"$gt\": 10000}}"
);
```

**Get all products (highly varied schemas):**
```csharp
await grid.PopulateFromMongoQuery(connectionString, "lawgrid_test", "products", "{}");
// Result: Grid with columns for ALL fields from electronics, furniture, books, software!
```

**Electronics products only:**
```csharp
await grid.PopulateFromMongoQuery(
    connectionString,
    "lawgrid_test",
    "products",
    "{\"category\": \"Electronics\"}"
);
```

**Employees with skills (only some have this field):**
```csharp
await grid.PopulateFromMongoQuery(
    connectionString,
    "lawgrid_test",
    "employees",
    "{\"skills\": {\"$exists\": true}}"
);
```

### All Three Method Signatures

```csharp
// Method 1: Simple async (returns bool)
bool success = await grid.PopulateFromMongoQuery(
    connectionString,
    "lawgrid_test",
    "employees",
    "{}"
);

// Method 2: Synchronous (returns bool)
bool success = grid.PopulateFromMongoQuerySync(
    connectionString,
    "lawgrid_test",
    "employees",
    "{}"
);

// Method 3: Detailed result (returns MongoQueryResult)
var result = await grid.PopulateFromMongoQueryAsync(
    connectionString,
    "lawgrid_test",
    "employees",
    "{}"
);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.DocumentCount} documents");
    Console.WriteLine($"Total unique fields: {result.TotalFieldCount}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

## MongoDB Filter Syntax

MongoDB uses JSON for queries. Here are common patterns:

### Simple Filters
```csharp
"{}"                                    // All documents
"{\"field\": \"value\"}"                // Exact match
"{\"number_field\": 100}"               // Number match
```

### Comparison Operators
```csharp
"{\"salary\": {\"$gt\": 10000}}"        // Greater than
"{\"salary\": {\"$gte\": 10000}}"       // Greater than or equal
"{\"salary\": {\"$lt\": 5000}}"         // Less than
"{\"salary\": {\"$lte\": 5000}}"        // Less than or equal
"{\"salary\": {\"$ne\": 5000}}"         // Not equal
```

### Field Existence
```csharp
"{\"field\": {\"$exists\": true}}"      // Has field
"{\"field\": {\"$exists\": false}}"     // Doesn't have field
```

### Multiple Conditions (AND)
```csharp
"{\"department_id\": 60, \"salary\": {\"$gt\": 5000}}"
```

### OR Conditions
```csharp
"{\"$or\": [{\"department_id\": 60}, {\"department_id\": 40}]}"
```

### Array Matching
```csharp
"{\"skills\": \"Python\"}"              // Array contains value
"{\"color_options\": {\"$size\": 3}}"   // Array size
```

### Nested Document Fields
```csharp
"{\"specifications.processor\": \"Intel i7\"}"
"{\"emergency_contact.relationship\": \"Spouse\"}"
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
docker-compose logs mongodb
```

### Connect to MongoDB Shell (mongosh)
```bash
docker exec -it lawgrid-mongodb mongosh -u testuser -p TestPassword123 --authenticationDatabase lawgrid_test lawgrid_test
```

Once connected, useful mongosh commands:
- `show collections` - List all collections
- `db.employees.find({})` - Get all employees
- `db.employees.countDocuments()` - Count documents
- `exit` - Quit mongosh

## Dynamic Schema Examples

### Example 1: Employees Collection

When you query the employees collection with `{}`, LAWgrid will create a grid with these columns (and more):

```
_id | employee_id | first_name | last_name | email | phone_number | hire_date | job_id | salary |
commission_pct | manager_id | department_id | skills | remote_work_eligible | certifications |
performance_rating | last_review_date | emergency_contact | previous_employers | contract_end_date |
hourly_rate | employment_type | hours_per_week
```

Employees that don't have certain fields will show **empty cells** for those columns.

### Example 2: Products Collection

Querying products collection shows incredible schema variety:

```
_id | product_id | product_name | category | price | stock_quantity | specifications | warranty_years |
color_options | switch_type | backlit | energy_star_rating | vesa_mount_compatible | material |
weight_capacity | adjustable_height | color | assembly_required | dimensions | electric_adjustment |
resolution | frame_rate | auto_focus | built_in_microphone | driver_size | frequency_response |
noise_cancelling | bluetooth_version | battery_life_hours | color_temperature | dimmable | author |
publisher | isbn | pages | publication_year | format | is_subscription | billing_period | platforms |
max_devices | cloud_storage_gb
```

This is the power of NoSQL + LAWgrid's dynamic field handling!

## Customizing the Setup

### Adding Your Own Data

Create additional JavaScript files in the `init-scripts/` directory. They will be executed in alphabetical order during container initialization.

Example: `init-scripts/02-custom-data.js`

```javascript
db = db.getSiblingDB('lawgrid_test');

db.employees.insertOne({
  employee_id: 200,
  first_name: 'Custom',
  last_name: 'Employee',
  email: 'custom@company.com',
  hire_date: new Date('2024-01-01'),
  salary: 50000,
  custom_field: 'Custom Value'
});
```

**Note:** Init scripts only run during first container creation. To re-run:
```bash
docker-compose down -v
docker-compose up -d
```

### Changing Passwords

Edit `docker-compose.yml` and change:
- `MONGO_INITDB_ROOT_PASSWORD` for admin
- Update the user password in `01-create-collections.js`

Then:
```bash
docker-compose down -v
docker-compose up -d
```

## MongoDB-Specific Features

### Arrays
MongoDB documents can contain arrays:
```javascript
{
  employee_id: 103,
  skills: ['C#', 'Python', 'SQL']
}
```

LAWgrid displays arrays as: `[C#, Python, SQL]`

### Nested Documents
MongoDB supports nested documents:
```javascript
{
  product_id: 1001,
  specifications: {
    processor: 'Intel i7',
    ram: '16GB'
  }
}
```

LAWgrid displays nested documents as JSON strings.

### ObjectId
MongoDB's `_id` field is displayed as a string representation of the ObjectId.

### Dates
MongoDB dates are displayed in `yyyy-MM-dd HH:mm:ss` format.

## Troubleshooting

### Container won't start
- Ensure port 27017 is not in use: `lsof -i :27017` (Linux/Mac) or `netstat -an | findstr 27017` (Windows)
- Check Docker logs: `docker-compose logs mongodb`
- Try: `docker-compose down -v` then `docker-compose up -d`

### Connection refused
- Wait for healthcheck to pass: `docker-compose ps`
- Check container is running: `docker ps`
- Verify connection string has correct username/password

### Authentication errors
- Ensure you're using the correct username (`testuser`) and password (`TestPassword123`)
- For admin operations, use `admin` user
- Check authentication database in connection string

### "No documents found" but data exists
- Verify collection name is correct (case-sensitive!)
- Check filter syntax (must be valid JSON)
- Test query in mongosh first

### Grid shows unexpected columns
- This is normal! LAWgrid shows ALL fields from ALL documents
- Documents with varied schemas will create many columns
- Use filters to narrow down results if needed

## Performance Tips

- Use filters to limit document count for large collections
- Index fields used in filters (already done for sample data)
- MongoDB performs well with denormalized data (nested documents)
- Connection string supports connection pooling for production use

## Comparison with SQL Databases

| Feature | SQL (Relational) | MongoDB (NoSQL) |
|---------|-----------------|-----------------|
| Schema | Fixed, must match | Flexible, can vary |
| Empty Fields | NULL values | Field doesn't exist |
| LAWgrid Handling | Columns predefined | **Dynamic column creation** |
| Joins | SQL JOINs | Embedded documents or `$lookup` |
| Query Language | SQL | JSON filters |

## Resources

- [MongoDB Documentation](https://www.mongodb.com/docs/manual/)
- [MongoDB .NET Driver](https://www.mongodb.com/docs/drivers/csharp/)
- [MongoDB Query Operators](https://www.mongodb.com/docs/manual/reference/operator/query/)
- [LAWgrid Repository](https://github.com/your-repo/LAWgrid)

## License

MongoDB Community Edition is free and open source (SSPL license). By using this Docker image, you accept MongoDB's license terms.
