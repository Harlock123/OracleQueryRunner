using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region PostgreSQL Population Methods

    /// <summary>
    /// Populates the grid with results from a PostgreSQL database query
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string</param>
    /// <param name="postgresQuery">PostgreSQL query to execute</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> PopulateFromPostgresQuery(string connectionString, string postgresQuery)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(postgresQuery))
            throw new ArgumentException("PostgreSQL query cannot be null or empty", nameof(postgresQuery));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(postgresQuery, connection);
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
        catch (NpgsqlException ex)
        {
            // Log or handle PostgreSQL-specific errors
            System.Diagnostics.Debug.WriteLine($"PostgreSQL Error: {ex.Message}");
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
    /// Populates the grid with results from a PostgreSQL database query (synchronous version)
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string</param>
    /// <param name="postgresQuery">PostgreSQL query to execute</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool PopulateFromPostgresQuerySync(string connectionString, string postgresQuery)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(postgresQuery))
            throw new ArgumentException("PostgreSQL query cannot be null or empty", nameof(postgresQuery));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(postgresQuery, connection);
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
        catch (NpgsqlException ex)
        {
            // Log or handle PostgreSQL-specific errors
            System.Diagnostics.Debug.WriteLine($"PostgreSQL Error: {ex.Message}");
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
    /// Populates the grid with results from a PostgreSQL database query with detailed result information
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string</param>
    /// <param name="postgresQuery">PostgreSQL query to execute</param>
    /// <returns>PostgresQueryResult with success status, error message, and row count</returns>
    public async Task<PostgresQueryResult> PopulateFromPostgresQueryAsync(string connectionString, string postgresQuery)
    {
        var result = new PostgresQueryResult();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            result.Success = false;
            result.ErrorMessage = "Connection string cannot be null or empty";
            return result;
        }

        if (string.IsNullOrWhiteSpace(postgresQuery))
        {
            result.Success = false;
            result.ErrorMessage = "PostgreSQL query cannot be null or empty";
            return result;
        }

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(postgresQuery, connection);
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
        catch (NpgsqlException ex)
        {
            result.Success = false;
            result.ErrorMessage = $"PostgreSQL Error: {ex.Message}";
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
