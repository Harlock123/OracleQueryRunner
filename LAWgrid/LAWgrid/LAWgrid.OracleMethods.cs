using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Oracle Population Methods

    /// <summary>
    /// Populates the grid with results from an Oracle database query
    /// </summary>
    /// <param name="connectionString">Oracle connection string</param>
    /// <param name="oracleQuery">Oracle query to execute</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> PopulateFromOracleQuery(string connectionString, string oracleQuery)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(oracleQuery))
            throw new ArgumentException("Oracle query cannot be null or empty", nameof(oracleQuery));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new OracleCommand(oracleQuery, connection);
            command.CommandTimeout = 30; // 30 seconds timeout

            await using var reader = await command.ExecuteReaderAsync();

            // Get column names from the result set
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            // Read all rows
            while (await reader.ReadAsync())
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = columnNames[i];
                    object value = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i);

                    // Convert value to string for display
                    expando[columnName] = value?.ToString() ?? string.Empty;
                }

                _items.Add(expando);
            }

            // Reset scroll positions and render on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _gridXShift = 0;
                _gridYShift = 0;
                ReRender();
            });

            return true;
        }
        catch (OracleException ex)
        {
            // Log or handle Oracle-specific errors
            System.Diagnostics.Debug.WriteLine($"Oracle Error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle general errors
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Populates the grid with results from an Oracle database query (synchronous version)
    /// </summary>
    /// <param name="connectionString">Oracle connection string</param>
    /// <param name="oracleQuery">Oracle query to execute</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool PopulateFromOracleQuerySync(string connectionString, string oracleQuery)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(oracleQuery))
            throw new ArgumentException("Oracle query cannot be null or empty", nameof(oracleQuery));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            using var connection = new OracleConnection(connectionString);
            connection.Open();

            using var command = new OracleCommand(oracleQuery, connection);
            command.CommandTimeout = 30; // 30 seconds timeout

            using var reader = command.ExecuteReader();

            // Get column names from the result set
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            // Read all rows
            while (reader.Read())
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = columnNames[i];
                    object value = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i);

                    // Convert value to string for display
                    expando[columnName] = value?.ToString() ?? string.Empty;
                }

                _items.Add(expando);
            }

            // Reset scroll positions and render on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                _gridXShift = 0;
                _gridYShift = 0;
                ReRender();
            });

            return true;
        }
        catch (OracleException ex)
        {
            // Log or handle Oracle-specific errors
            System.Diagnostics.Debug.WriteLine($"Oracle Error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle general errors
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Populates the grid with results from an Oracle database query with detailed result information
    /// </summary>
    /// <param name="connectionString">Oracle connection string</param>
    /// <param name="oracleQuery">Oracle query to execute</param>
    /// <returns>OracleQueryResult with success status, error message, and row count</returns>
    public async Task<OracleQueryResult> PopulateFromOracleQueryAsync(string connectionString, string oracleQuery)
    {
        var result = new OracleQueryResult();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            result.Success = false;
            result.ErrorMessage = "Connection string cannot be null or empty";
            return result;
        }

        if (string.IsNullOrWhiteSpace(oracleQuery))
        {
            result.Success = false;
            result.ErrorMessage = "Oracle query cannot be null or empty";
            return result;
        }

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new OracleCommand(oracleQuery, connection);
            command.CommandTimeout = 30; // 30 seconds timeout

            await using var reader = await command.ExecuteReaderAsync();

            // Get column names from the result set
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            // Read all rows
            int rowCount = 0;
            while (await reader.ReadAsync())
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = columnNames[i];
                    object value = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i);

                    // Convert value to string for display
                    expando[columnName] = value?.ToString() ?? string.Empty;
                }

                _items.Add(expando);
                rowCount++;
            }

            // Reset scroll positions and render on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _gridXShift = 0;
                _gridYShift = 0;
                ReRender();
            });

            result.Success = true;
            result.RowCount = rowCount;
            result.ErrorMessage = string.Empty;
            return result;
        }
        catch (OracleException ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Oracle Error: {ex.Message}\nError Number: {ex.Number}";
            System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
            return result;
        }
    }

    #endregion
}
