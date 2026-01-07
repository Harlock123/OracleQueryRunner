# LAWgrid

A custom data grid control built for Avalonia UI - a cross-platform .NET UI framework.

## Overview

LAWgrid is a highly customizable user control that displays tabular data in a grid format with extensive customization options and direct integration with multiple database systems including SQL Server, Oracle, MySQL, PostgreSQL, MongoDB (NoSQL), and DB2.

## Features

### Data Population
- **SQL Server Integration**: Direct population from SQL Server queries
- **Oracle Integration**: Direct population from Oracle database queries
- **MySQL Integration**: Direct population from MySQL database queries
- **PostgreSQL Integration**: Direct population from PostgreSQL database queries
- **MongoDB Integration**: Direct population from MongoDB (NoSQL) with **dynamic schema handling**
  - Automatically detects all fields across documents with different schemas
  - Creates unified grid with all fields as columns
  - Fills empty cells for missing fields in documents
- **DB2 Integration**: Direct population from IBM DB2 database queries
- **DataTable Support**: Populate from .NET DataTable objects
- **Test Data Support**: Built-in test data population for development and testing

### Data Export
- **Excel Export**: Export grid data to XLSX format with full formatting support
  - Preserves colors (backgrounds and text)
  - Maintains font sizes and styles
  - Supports GreenBar alternating row colors
  - Auto-fits columns to content
  - Includes title and header rows
  - Save file dialog with customizable filename

### Visual Customization
- Configurable colors for backgrounds, cells, headers, and highlights
- **GreenBar Mode**: Alternating row colors for improved readability
- Customizable fonts and typefaces (title, header, and cell content)
- Adjustable font sizes for different grid sections
- Configurable cell padding (width and height)
- Cell outline customization

### Interactive Features
- **Mouse Tracking**: Real-time mouse position tracking with optional crosshairs
- **Cell Highlighting**: Visual feedback when hovering over cells
- **Cell Selection**: Support for selecting individual items
- **Double-Click Detection**: Built-in double-click timer for handling user interactions
- **Context Menu**: Right-click menu with font size adjustment and Excel export options
- **Scrolling**: Both horizontal and vertical scrollbars with configurable scroll multiplier

### Layout & Rendering
- **Auto-sizing**: Cells can automatically resize to fit their contents
- **Dynamic Dimensions**: Configurable grid width and height
- **Viewport Management**: Grid shift support for X and Y coordinates
- **Clipping**: Content properly clipped to grid boundaries
- **Suspend Rendering**: Option to temporarily suspend rendering for batch updates

### Built-in Icons
- Checkmark icon
- Red X icon
- Folder icon
- File icon

### Image Export
- **PNG Export**: Save grid visual to PNG format
- **JPEG Export**: Save grid visual to JPEG with quality control
- **BMP Export**: Save grid visual to BMP format
- **Desktop Quick Save**: One-click save to Desktop
- **Clipboard Support**: Copy grid visual to clipboard (cross-platform)

## Project Structure

```
LAWgrid/
├── LAWgrid/                        # Main grid control library
│   ├── LAWgrid.axaml               # XAML layout definition
│   ├── LAWgrid.axaml.cs            # Main partial class (constructor, variables, events)
│   ├── LAWgrid.Properties.cs       # Partial class - Public properties
│   ├── LAWgrid.Rendering.cs        # Partial class - Rendering methods
│   ├── LAWgrid.SqlMethods.cs       # Partial class - SQL Server population methods
│   ├── LAWgrid.OracleMethods.cs    # Partial class - Oracle population methods
│   ├── LAWgrid.MySqlMethods.cs     # Partial class - MySQL population methods
│   ├── LAWgrid.PostgresMethods.cs  # Partial class - PostgreSQL population methods
│   ├── LAWgrid.MongoMethods.cs     # Partial class - MongoDB (NoSQL) population methods
│   ├── LAWgrid.Db2Methods.cs       # Partial class - DB2 population methods
│   ├── LAWgrid.DataPopulation.cs   # Partial class - Data population methods
│   ├── LAWgrid.ExcelMethods.cs     # Partial class - Excel export methods
│   ├── LAWgrid.EventHandlers.cs    # Partial class - Event handlers
│   ├── LAWgrid.PublicMethods.cs    # Partial class - Public utility methods
│   ├── LAWgrid.PrivateMethods.cs   # Partial class - Private helper methods
│   ├── LAWgrid.HelperClasses.cs    # Helper classes and data models
│   └── LAWgrid.csproj              # Project file
├── GridTestBed/                    # Test application
│   ├── App.axaml
│   ├── App.axaml.cs
│   ├── MainWindow.axaml
│   ├── MainWindow.axaml.cs
│   ├── Program.cs
│   └── GridTestBed.csproj
└── LAWgrid.sln                     # Solution file
```

### LAWgrid Partial Class Organization

The LAWgrid control is organized into multiple partial class files for better maintainability and code organization:

- **LAWgrid.axaml.cs** - Core class with constructor, private variables, and event declarations
- **LAWgrid.Properties.cs** - All public properties with DefaultValue attributes
- **LAWgrid.Rendering.cs** - Main rendering logic and ReRender() method
- **LAWgrid.SqlMethods.cs** - SQL Server query population methods
- **LAWgrid.OracleMethods.cs** - Oracle database query population methods
- **LAWgrid.MySqlMethods.cs** - MySQL database query population methods
- **LAWgrid.PostgresMethods.cs** - PostgreSQL database query population methods
- **LAWgrid.MongoMethods.cs** - MongoDB (NoSQL) database query population methods with dynamic schema handling
- **LAWgrid.Db2Methods.cs** - DB2 database query population methods
- **LAWgrid.DataPopulation.cs** - Array and test data population methods
- **LAWgrid.ExcelMethods.cs** - Excel export methods with formatting
- **LAWgrid.EventHandlers.cs** - Mouse, pointer, scroll, and context menu handlers
- **LAWgrid.PublicMethods.cs** - Public utility methods for grid manipulation
- **LAWgrid.PrivateMethods.cs** - Private helper methods
- **LAWgrid.HelperClasses.cs** - Supporting classes and data models

## Dependencies

- **Avalonia** (v11.2.2): Cross-platform UI framework
- **Avalonia.Desktop** (v11.2.2): Desktop-specific Avalonia components
- **Microsoft.Data.SqlClient** (v6.0.1): SQL Server data access
- **Oracle.ManagedDataAccess.Core** (v23.7.0): Oracle database data access
- **MySqlConnector** (v2.4.0): MySQL database data access (high-performance async driver)
- **Npgsql** (v9.0.2): PostgreSQL database data access (official .NET driver)
- **MongoDB.Driver** (v3.0.0): MongoDB (NoSQL) database data access with dynamic schema support
- **Net.IBM.Data.Db2** (v9.0.0.400): IBM DB2 database data access
- **ClosedXML** (v0.104.2): Excel file generation (MIT License - fully free!)
- **SkiaSharp** (v2.88.9): Image encoding for JPEG/BMP export
- **Target Framework**: .NET 9.0

## Usage Examples

### Database Population

#### SQL Server
```csharp
// Async with detailed result information
string connectionString = "Server=myserver;Database=MyDB;User Id=username;Password=password;";
string query = "SELECT * FROM Customers";
var result = await TheGridInTest.PopulateFromSqlQueryAsync(connectionString, query);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.RowCount} rows");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}

// Synchronous version
TheGridInTest.PopulateFromSqlQuerySync(connectionString, query);

// Simple async (returns bool)
await TheGridInTest.PopulateFromSqlQuery(connectionString, query);
```

#### Oracle
```csharp
// Async with detailed result information
string connectionString = "Data Source=myserver:1521/ORCL;User Id=username;Password=password;";
string query = "SELECT * FROM EMPLOYEES";
var result = await TheGridInTest.PopulateFromOracleQueryAsync(connectionString, query);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.RowCount} rows");
}

// Synchronous version
TheGridInTest.PopulateFromOracleQuerySync(connectionString, query);

// Simple async (returns bool)
await TheGridInTest.PopulateFromOracleQuery(connectionString, query);
```

#### MySQL
```csharp
// Async with detailed result information
string connectionString = "Server=myserver;Database=mydb;User=root;Password=password;";
string query = "SELECT * FROM customers";
var result = await TheGridInTest.PopulateFromMySqlQueryAsync(connectionString, query);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.RowCount} rows");
}

// Synchronous version
TheGridInTest.PopulateFromMySqlQuerySync(connectionString, query);

// Simple async (returns bool)
await TheGridInTest.PopulateFromMySqlQuery(connectionString, query);
```

#### PostgreSQL
```csharp
// Async with detailed result information
string connectionString = "Host=myserver;Database=mydb;Username=postgres;Password=password;";
string query = "SELECT * FROM customers";
var result = await TheGridInTest.PopulateFromPostgresQueryAsync(connectionString, query);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.RowCount} rows");
}

// Synchronous version
TheGridInTest.PopulateFromPostgresQuerySync(connectionString, query);

// Simple async (returns bool)
await TheGridInTest.PopulateFromPostgresQuery(connectionString, query);
```

#### MongoDB (NoSQL)
```csharp
// MongoDB uses collection name and JSON filter instead of SQL query
// Async with detailed result information
string connectionString = "mongodb://username:password@myserver:27017/mydb";
string databaseName = "mydb";
string collectionName = "customers";
string filter = "{}"; // Empty filter = all documents

var result = await TheGridInTest.PopulateFromMongoQueryAsync(
    connectionString,
    databaseName,
    collectionName,
    filter
);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.DocumentCount} documents");
    Console.WriteLine($"Total unique fields: {result.TotalFieldCount}");
    // MongoDB automatically handles documents with different schemas!
}

// Filter examples
string itDeptFilter = "{\"department_id\": 60}";  // Specific department
string highEarnersFilter = "{\"salary\": {\"$gt\": 50000}}";  // Salary > 50000
string withSkillsFilter = "{\"skills\": {\"$exists\": true}}";  // Has skills field

// Synchronous version
TheGridInTest.PopulateFromMongoQuerySync(connectionString, databaseName, collectionName, filter);

// Simple async (returns bool)
await TheGridInTest.PopulateFromMongoQuery(connectionString, databaseName, collectionName, filter);
```

**MongoDB Dynamic Schema Handling:**
Unlike SQL databases, MongoDB documents in the same collection can have different fields. LAWgrid automatically:
- Collects all unique field names across all documents
- Creates a unified grid with all fields as columns
- Fills empty cells for documents missing certain fields

This allows you to query collections with varied schemas and see all data in one grid!

#### DB2
```csharp
// Async with detailed result information
string connectionString = "Server=myserver:50000;Database=SAMPLE;UID=db2admin;PWD=password;";
string query = "SELECT * FROM EMPLOYEE";
var result = await TheGridInTest.PopulateFromDb2QueryAsync(connectionString, query);

if (result.Success)
{
    Console.WriteLine($"Loaded {result.RowCount} rows");
}

// Synchronous version
TheGridInTest.PopulateFromDb2QuerySync(connectionString, query);

// Simple async (returns bool)
await TheGridInTest.PopulateFromDb2Query(connectionString, query);
```

#### DataTable
```csharp
// Populate from DataTable
DataTable dt = new DataTable();
dt.Columns.Add("ID", typeof(int));
dt.Columns.Add("Name", typeof(string));
dt.Rows.Add(1, "John Doe");
dt.Rows.Add(2, "Jane Smith");
TheGridInTest.PopulateFromDataTable(dt);
```

#### Test Data
```csharp
// Populate with test data
TheGridInTest.TestPopulate();
```

### Excel Export

```csharp
// Export with save file dialog (recommended)
// Default filename: EXCELExport_MM-DD-YYYY.xlsx
string? filePath = await TheGridInTest.ExportToExcelWithDialog();

if (filePath != null)
{
    Console.WriteLine($"Excel file saved to: {filePath}");
}

// Export to specific file path
bool success = TheGridInTest.ExportToExcel("/path/to/output.xlsx");
```

The Excel export includes:
- Title row with merged cells and title formatting
- Column headers with bold text and header colors
- Data rows with proper formatting
- Background colors (supports GreenBar mode)
- Text colors
- Font sizes matching the grid
- Auto-fitted columns
- Cell borders

### Visual Customization

```csharp
// Enable GreenBar mode with alternating row colors
TheGridInTest.GreenBarMode = true;
TheGridInTest.GreenBarColor1 = Brushes.White;
TheGridInTest.GreenBarColor2 = Brushes.PaleGreen;

// Or toggle it programmatically
TheGridInTest.ToggleGreenBarMode();
```

### Image Export

```csharp
// Save to Desktop (quick and easy)
string? filePath = TheGridInTest.SaveGridToDesktop();

// Save to specific location as PNG
TheGridInTest.SaveGridAsPng("/path/to/output.png");

// Save as JPEG with quality control (0-100)
TheGridInTest.SaveGridAsJpg("/path/to/output.jpg", quality: 90);

// Save as BMP
TheGridInTest.SaveGridAsBmp("/path/to/output.bmp");

// Copy to clipboard (cross-platform)
await TheGridInTest.CopyGridToClipboardAsync();
```

## Configuration Options

### Title & Header
- `GridTitle`: Title text displayed at the top
- `GridTitleFontSize`: Font size for the title (default: 20)
- `GridTitleHeight`: Height of the title area (default: 10)
- `GridHeaderFontSize`: Font size for column headers (default: 14)

### Cell Appearance
- `GridFontSize`: Font size for cell content (default: 12)
- `CellPaddingWidth`: Horizontal padding (default: 10)
- `CellPaddingHeight`: Vertical padding (default: 10)
- `AutosizeCellsToContents`: Enable/disable auto-sizing (default: true)

### Colors & Brushes
- `GridBackground`: Background color (default: Cornsilk)
- `GridCellBrush`: Cell background color (default: Wheat)
- `GridCellOutline`: Cell border color (default: Black)
- `GridCellContentBrush`: Cell text color (default: Black)
- `GridCellHighlightBrush`: Highlighted cell color (default: LightBlue)
- `GridSelectedItemBrush`: Selected item color (default: AliceBlue)
- `GridTitleBackground`: Title area background (default: Blue)
- `GridTitleBrush`: Title text color (default: White)
- `GridHeaderBackground`: Header area background (default: Cyan)
- `GridHeaderBrush`: Header text color (default: Black)
- `GreenBarMode`: Enable alternating row colors (default: false)
- `GreenBarColor1`: First alternating row color (default: White)
- `GreenBarColor2`: Second alternating row color (default: PaleGreen)

### Interaction
- `ShowCrossHairs`: Enable/disable crosshair cursor (default: true)
- `CrossHairBrush`: Crosshair color (default: Red)
- `ScrollMultiplier`: Scroll speed multiplier (default: 3)

## Context Menu Options

Right-click on the grid to access:
- **Fonts Larger**: Increase font sizes
- **Fonts Smaller**: Decrease font sizes
- **Export to Excel**: Export grid data to XLSX format with formatting

**Alt + MouseWheel** will also increase and decrease displayed font sizes

## API Reference

### Database Population Methods

All database population methods come in three variants:
1. **Simple Async** (`PopulateFrom[Database]Query`) - Returns `Task<bool>`
2. **Synchronous** (`PopulateFrom[Database]QuerySync`) - Returns `bool`
3. **Detailed Async** (`PopulateFrom[Database]QueryAsync`) - Returns `Task<[Database]QueryResult>` with success status, row count, and error messages

#### SQL Server Methods
- `PopulateFromSqlQuery(string connectionString, string sqlQuery)` → `Task<bool>`
- `PopulateFromSqlQuerySync(string connectionString, string sqlQuery)` → `bool`
- `PopulateFromSqlQueryAsync(string connectionString, string sqlQuery)` → `Task<SqlQueryResult>`

#### Oracle Methods
- `PopulateFromOracleQuery(string connectionString, string oracleQuery)` → `Task<bool>`
- `PopulateFromOracleQuerySync(string connectionString, string oracleQuery)` → `bool`
- `PopulateFromOracleQueryAsync(string connectionString, string oracleQuery)` → `Task<OracleQueryResult>`

#### MySQL Methods
- `PopulateFromMySqlQuery(string connectionString, string mySqlQuery)` → `Task<bool>`
- `PopulateFromMySqlQuerySync(string connectionString, string mySqlQuery)` → `bool`
- `PopulateFromMySqlQueryAsync(string connectionString, string mySqlQuery)` → `Task<MySqlQueryResult>`

#### PostgreSQL Methods
- `PopulateFromPostgresQuery(string connectionString, string postgresQuery)` → `Task<bool>`
- `PopulateFromPostgresQuerySync(string connectionString, string postgresQuery)` → `bool`
- `PopulateFromPostgresQueryAsync(string connectionString, string postgresQuery)` → `Task<PostgresQueryResult>`

#### MongoDB Methods (NoSQL)
- `PopulateFromMongoQuery(string connectionString, string databaseName, string collectionName, string filter)` → `Task<bool>`
- `PopulateFromMongoQuerySync(string connectionString, string databaseName, string collectionName, string filter)` → `bool`
- `PopulateFromMongoQueryAsync(string connectionString, string databaseName, string collectionName, string filter)` → `Task<MongoQueryResult>`

**Note:** MongoDB methods use a different signature with 4 parameters:
- `connectionString`: MongoDB connection string
- `databaseName`: Database name
- `collectionName`: Collection name (like a table in SQL)
- `filter`: JSON filter string (e.g., `"{}"` for all, `"{\"field\": \"value\"}"` for filtered)

**MongoQueryResult** includes:
- `Success`: Operation success status
- `ErrorMessage`: Error details if failed
- `DocumentCount`: Number of documents retrieved
- `TotalFieldCount`: Total unique fields across all documents (for dynamic schema)

#### DB2 Methods
- `PopulateFromDb2Query(string connectionString, string db2Query)` → `Task<bool>`
- `PopulateFromDb2QuerySync(string connectionString, string db2Query)` → `bool`
- `PopulateFromDb2QueryAsync(string connectionString, string db2Query)` → `Task<Db2QueryResult>`

#### DataTable Method
- `PopulateFromDataTable(DataTable dataTable)` → `bool`

### Excel Export Methods

#### `ExportToExcelWithDialog()`
Shows a save file dialog and exports the grid to an Excel file.
- **Returns**: `Task<string?>` - The full path to the saved file, or null if cancelled/failed
- **Default Filename**: `EXCELExport_MM-DD-YYYY.xlsx`
- **Features**:
  - Preserves all colors (title, headers, cells, GreenBar)
  - Maintains font sizes and styles
  - Auto-fits columns
  - Adds cell borders
  - Worksheet named "EXCELEXPORT"
- **Example**:
```csharp
string? path = await TheGridInTest.ExportToExcelWithDialog();
if (path != null)
{
    Console.WriteLine($"Exported to: {path}");
}
```

#### `ExportToExcel(string filePath)`
Exports the grid to an Excel file at the specified path.
- **Parameters**:
  - `filePath`: Full path where the Excel file should be saved
- **Returns**: `bool` - True if successful, false otherwise
- **Example**:
```csharp
bool success = TheGridInTest.ExportToExcel("/home/user/Documents/report.xlsx");
```

### Visual Customization Methods

#### `ToggleGreenBarMode()`
Toggles the GreenBarMode on or off (alternating row colors).
- **Returns**: void
- **Example**: `TheGridInTest.ToggleGreenBarMode();`

### Image Export Methods

#### `SaveGridToDesktop()`
Saves the grid visual as a PNG file to the user's Desktop.
- **Returns**: `string?` - The full path to the saved file, or null if failed
- **Example**: `string? path = TheGridInTest.SaveGridToDesktop();`

#### `SaveGridAsPng(string filePath)`
Saves the grid visual as a PNG file to a specific location.
- **Parameters**:
  - `filePath`: The file path where to save the PNG
- **Returns**: `bool` - True if successful, false otherwise
- **Example**: `TheGridInTest.SaveGridAsPng("/path/to/output.png");`

#### `SaveGridAsJpg(string filePath, int quality = 90)`
Saves the grid visual as a JPEG file with configurable quality.
- **Parameters**:
  - `filePath`: The file path where to save the JPEG
  - `quality`: JPEG quality (0-100, default: 90)
- **Returns**: `bool` - True if successful, false otherwise
- **Example**: `TheGridInTest.SaveGridAsJpg("/path/to/output.jpg", quality: 85);`

#### `SaveGridAsBmp(string filePath)`
Saves the grid visual as a BMP file.
- **Parameters**:
  - `filePath`: The file path where to save the BMP
- **Returns**: `bool` - True if successful, false otherwise
- **Example**: `TheGridInTest.SaveGridAsBmp("/path/to/output.bmp");`

#### `CopyGridToClipboardAsync()`
Captures the grid visual as a PNG and copies it to the clipboard (cross-platform).
- **Returns**: `Task<bool>` - True if successful, false otherwise
- **Example**: `bool success = await TheGridInTest.CopyGridToClipboardAsync();`
- **Note**: This is an async method and must be awaited

## Development

The GridTestBed project serves as a development and testing environment for the LAWgrid control. It includes multiple test buttons for different functionality testing scenarios.

## Platform Support

As an Avalonia-based control, LAWgrid supports:
- Windows
- Linux
- macOS

## Database Support

LAWgrid provides native support for the following database systems:
- **SQL Server** - Microsoft's enterprise database
- **Oracle** - Oracle Database (all editions)
- **MySQL** - Popular open-source database (via high-performance MySqlConnector)
- **PostgreSQL** - Advanced open-source database (via official Npgsql driver)
- **MongoDB** - NoSQL document database with **dynamic schema handling**
  - Automatically handles collections with varied document structures
  - Detects all unique fields across documents
  - Creates unified grid view for heterogeneous data
- **DB2** - IBM's enterprise database
- **Any ADO.NET Source** - via DataTable support

All database methods handle connection management, error handling, and provide both synchronous and asynchronous variants for maximum flexibility.

### Docker Test Environments

Complete Docker environments with sample data are available for testing all database integrations:
- Oracle XE 21c
- PostgreSQL 16
- MySQL 8.0
- MongoDB 7.0 (with varied schema examples)
- IBM DB2 11.5

See `DOCKER_DATABASES_README.md` for setup instructions and test data.

## License

MIT

## Author

Lonnie Watson
