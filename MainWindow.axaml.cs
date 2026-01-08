using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Oracle.ManagedDataAccess.Client;

namespace OracleQueryRunner;

public partial class MainWindow : Window
{
    private const string SettingsFileName = "oracle_query_settings.json";
    private string SettingsFilePath => Path.Combine(
        Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory,
        SettingsFileName);

    private string _activeConnectionString = string.Empty;
    private CheckBox[] _connCheckBoxes = null!;
    private TextBlock[] _connLabels = null!;
    private string[] _connectionStrings = null!;

    public MainWindow()
    {
        InitializeComponent();
        InitializeConnectionControls();
        LoadSettings();
    }

    private void InitializeConnectionControls()
    {
        _connCheckBoxes = new[] { ConnCheck1, ConnCheck2, ConnCheck3, ConnCheck4 };
        _connLabels = new[] { ConnLabel1, ConnLabel2, ConnLabel3, ConnLabel4 };
        _connectionStrings = new string[4];
    }

    private void ConnButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;

        int index = button.Name switch
        {
            "ConnButton1" => 0,
            "ConnButton2" => 1,
            "ConnButton3" => 2,
            "ConnButton4" => 3,
            _ => -1
        };

        if (index < 0) return;

        // Check if this connection is configured
        if (string.IsNullOrWhiteSpace(_connectionStrings[index]))
        {
            StatusText.Text = $"Connection '{_connLabels[index].Text}' is not configured";
            return;
        }

        // Update checkboxes - uncheck all, then check the selected one
        for (int i = 0; i < _connCheckBoxes.Length; i++)
        {
            _connCheckBoxes[i].IsChecked = (i == index);
        }

        _activeConnectionString = _connectionStrings[index];
        TablesListBox.ItemsSource = null;
        StatusText.Text = $"Selected: {_connLabels[index].Text}";
    }

    private async void ExecuteButton_Click(object? sender, RoutedEventArgs e)
    {
        var query = QueryTextBox.Text;

        if (string.IsNullOrWhiteSpace(_activeConnectionString))
        {
            StatusText.Text = "Error: Please select a connection";
            return;
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            StatusText.Text = "Error: Please enter a SQL query";
            return;
        }

        try
        {
            ExecuteButton.IsEnabled = false;
            StatusText.Text = "Executing query...";

            var result = await ResultsGrid.PopulateFromOracleQueryAsync(_activeConnectionString, query);

            if (result.Success)
            {
                StatusText.Text = $"Query completed successfully. {result.RowCount} row(s) returned.";
            }
            else
            {
                StatusText.Text = $"Query failed: {result.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            ExecuteButton.IsEnabled = true;
        }
    }

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        ResultsGrid.Items = new System.Collections.Generic.List<object>();
        StatusText.Text = "Results cleared";
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize(json, AppJsonContext.Default.AppSettings);
                if (settings?.Connections != null)
                {
                    for (int i = 0; i < Math.Min(settings.Connections.Count, 4); i++)
                    {
                        var conn = settings.Connections[i];
                        _connLabels[i].Text = conn.Label;
                        _connectionStrings[i] = conn.ConnectionString;
                    }
                }
                StatusText.Text = $"Settings loaded from: {SettingsFilePath}";
            }
            else
            {
                CreateDefaultSettings();
                StatusText.Text = $"Settings created at: {SettingsFilePath}";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Settings error: {ex.Message} | Path: {SettingsFilePath}";
        }
    }

    private void CreateDefaultSettings()
    {
        var settings = new AppSettings
        {
            Connections = new List<ConnectionPreset>
            {
                new() { Label = "Connection 1", ConnectionString = "" },
                new() { Label = "Connection 2", ConnectionString = "" },
                new() { Label = "Connection 3", ConnectionString = "" },
                new() { Label = "Connection 4", ConnectionString = "" }
            }
        };

        var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = AppJsonContext.Default };
        var json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(SettingsFilePath, json);
    }

    private async void FetchTablesButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_activeConnectionString))
        {
            StatusText.Text = "Error: Please select a connection";
            return;
        }

        try
        {
            FetchTablesButton.IsEnabled = false;
            StatusText.Text = "Fetching tables...";

            var tables = await Task.Run(() =>
            {
                var tableList = new List<string>();
                using var connection = new OracleConnection(_activeConnectionString);
                connection.Open();

                using var command = new OracleCommand(
                    "SELECT table_name FROM user_tables ORDER BY table_name",
                    connection);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tableList.Add(reader.GetString(0));
                }

                return tableList;
            });

            TablesListBox.ItemsSource = tables;
            StatusText.Text = $"Found {tables.Count} table(s)";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            FetchTablesButton.IsEnabled = true;
        }
    }

    private void TablesListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TablesListBox.SelectedItem is string tableName)
        {
            QueryTextBox.Text = $"SELECT * FROM {tableName} FETCH FIRST 100 ROWS ONLY";
        }
    }
}

public class ConnectionPreset
{
    public string Label { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}

public class AppSettings
{
    public List<ConnectionPreset> Connections { get; set; } = new();
}

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(List<ConnectionPreset>))]
[JsonSerializable(typeof(ConnectionPreset))]
internal partial class AppJsonContext : JsonSerializerContext
{
}
