# Oracle Query Runner

A desktop application for executing Oracle SQL queries and viewing results in a data grid.

## Features

- Four saved connection presets, picked with one click
- List the tables of the current schema and start a query from one
- Execute SQL queries with syntax highlighting
- View results in a sortable data grid (LAWgrid)

## Requirements

- .NET 9.0 SDK
- Oracle database access

## Getting Started

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

## Usage

1. On first run the app writes `oracle_query_settings.json` next to the executable, with four empty
   presets. Edit it to give each preset a label and an Oracle connection string, then restart the app:
   ```json
   { "Connections": [ { "Label": "Dev", "ConnectionString": "Data Source=myserver:1521/ORCL;User Id=username;Password=password;" }, ... ] }
   ```
   The file holds passwords in plain text; don't commit it.

2. Click a connection button to make it the active connection (its check box is ticked)

3. Optionally click **Fetch Tables** and pick a table to fill in a `SELECT ... FETCH FIRST 100 ROWS ONLY`

4. Write your SQL query in the query text box

5. Click **Execute Query** to run the query and display results

## Project Structure

```
OracleQueryRunner/
├── App.axaml              # Application definition
├── MainWindow.axaml       # Main window UI layout
├── MainWindow.axaml.cs    # Main window logic and the connection presets
└── Program.cs             # Application entry point
```

## Dependencies

- [Avalonia UI](https://avaloniaui.net/) 11.3.10 - Cross-platform .NET UI framework
- [LAWgrid](https://www.nuget.org/packages/LAWgrid) 1.0.2 (NuGet) - Custom data grid with Oracle, PostgreSQL, MySQL, MongoDB, and DB2 support
- [SyntaxColorizer](https://www.nuget.org/packages/SyntaxColorizer) 1.0.1 (NuGet) - Syntax-highlighting editor for the query box

## License

See LICENSE file for details.
