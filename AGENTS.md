# AGENTS.md

Notes for anyone, human or AI agent, changing this code.

## What this is

A small desktop tool for running SQL against Oracle databases and viewing the results in a grid. It is a C#
Avalonia 11.3 app on .NET 9, cross-platform. The results grid and the Oracle access come from the author's
`LAWgrid` NuGet package (which brings in `Oracle.ManagedDataAccess`); the query box's highlighting comes from
`SyntaxColorizer`.

## Commands

```bash
dotnet build OracleQueryRunner.sln                # needs the .NET 9 SDK (no global.json)
dotnet run                                        # from the repository root
```

There are no tests yet, and no run arguments.

## Layout

The whole app is a single project at the repository root.

- `MainWindow.axaml`: the layout. A tables list, four connection preset buttons with check marks, the SQL editor
  (`SyntaxColorizer`), Execute and Clear buttons, the `LAWgrid` results grid and a status line.
- `MainWindow.axaml.cs`: all the logic.
  - `ConnButton_Click` picks one of the four presets as the active connection.
  - `ExecuteButton_Click` runs the query through `ResultsGrid.PopulateFromOracleQueryAsync`.
  - `FetchTablesButton_Click` lists `user_tables` with `OracleConnection` on a background task; choosing a
    table fills in a `SELECT ... FETCH FIRST 100 ROWS ONLY`.
  - `LoadSettings` and `CreateDefaultSettings` read and write the presets. `AppSettings`, `ConnectionPreset` and
    the source-generated `AppJsonContext` are at the bottom of the file.
- `App.axaml`, `Program.cs`, `app.manifest`: Avalonia start-up boilerplate.

## Rules that keep it working

- **Settings live beside the executable.** `oracle_query_settings.json` is read from the folder of
  `Environment.ProcessPath`; a default with four empty presets is written on first run. It holds connection
  strings, passwords included, in plain text. Don't commit one; it is not listed in `.gitignore`.
- **Four presets, fixed.** The arrays of buttons, check boxes and labels are built in
  `InitializeConnectionControls`, and button names map to indexes in `ConnButton_Click`. Adding a preset means
  changing the XAML, that array and that switch together.
- **Keep JSON source-generated.** New settings types need a `[JsonSerializable]` entry on `AppJsonContext`.
- **Database work stays off the UI thread** (async grid call, `Task.Run` for the table list).
- The README is older than the code: it describes typing a connection string and a `LAWgrid/` folder, neither of
  which exists now. Update it with any change to how connections work.

## Making a change

- Build, then try it against a real Oracle database; there are no tests.
- Update `README.md` to match.

There is no CI workflow.
