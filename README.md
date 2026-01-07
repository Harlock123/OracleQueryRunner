# Oracle Query Runner

A desktop application for executing Oracle SQL queries and viewing results in a data grid.

## Features

- Connect to Oracle databases using a connection string
- Execute SQL queries with syntax highlighting
- View results in a sortable data grid (LAWgrid)
- Save connection settings for quick access
- Password masking for connection strings

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

1. Enter your Oracle connection string in the format:
   ```
   Data Source=myserver:1521/ORCL;User Id=username;Password=password;
   ```

2. Write your SQL query in the query text box

3. Click **Execute Query** to run the query and display results

4. Use **Save Connection** to persist your connection string for future sessions

## Project Structure

```
OracleQueryRunner/
├── App.axaml              # Application definition
├── MainWindow.axaml       # Main window UI layout
├── MainWindow.axaml.cs    # Main window logic
├── Program.cs             # Application entry point
└── LAWgrid/               # Data grid component library
```

## Dependencies

- [Avalonia UI](https://avaloniaui.net/) 11.3.10 - Cross-platform .NET UI framework
- LAWgrid - Custom data grid with Oracle, PostgreSQL, MySQL, MongoDB, and DB2 support

## License

See LICENSE file for details.
