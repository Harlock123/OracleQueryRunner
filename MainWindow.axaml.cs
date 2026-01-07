using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace OracleQueryRunner;

public partial class MainWindow : Window
{
    private const string SettingsFileName = "oracle_query_settings.json";
    private string SettingsFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OracleQueryRunner",
        SettingsFileName);

    public MainWindow()
    {
        InitializeComponent();
        LoadSettings();
    }

    private async void ExecuteButton_Click(object? sender, RoutedEventArgs e)
    {
        var connectionString = ConnectionStringTextBox.Text;
        var query = QueryTextBox.Text;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            StatusText.Text = "Error: Please enter a connection string";
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

            var result = await ResultsGrid.PopulateFromOracleQueryAsync(connectionString, query);

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

    private void ShowPasswordToggle_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggle)
        {
            ConnectionStringTextBox.RevealPassword = toggle.IsChecked ?? false;
            toggle.Content = (toggle.IsChecked ?? false) ? "Hide" : "Show";
        }
    }

    private void SaveSettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            SaveSettings();
            StatusText.Text = "Connection settings saved";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Failed to save settings: {ex.Message}";
        }
    }

    private void SaveSettings()
    {
        var settings = new AppSettings
        {
            ConnectionString = ConnectionStringTextBox.Text ?? string.Empty
        };

        var directory = Path.GetDirectoryName(SettingsFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsFilePath, json);
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    ConnectionStringTextBox.Text = settings.ConnectionString;
                }
            }
        }
        catch
        {
            // Ignore errors when loading settings
        }
    }
}

public class AppSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}
