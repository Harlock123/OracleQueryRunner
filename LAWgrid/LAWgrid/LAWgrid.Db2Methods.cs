using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBM.Data.Db2;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region DB2 Population Methods

    /// <summary>
    /// Populates the grid with results from a DB2 database query
    /// </summary>
    /// <param name="connectionString">DB2 connection string</param>
    /// <param name="db2Query">DB2 query to execute</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> PopulateFromDb2Query(string connectionString, string db2Query)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(db2Query))
            throw new ArgumentException("DB2 query cannot be null or empty", nameof(db2Query));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            await using var connection = new DB2Connection(connectionString);
            await connection.OpenAsync();

            await using var command = new DB2Command(db2Query, connection);
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
        catch (DB2Exception ex)
        {
            // Log or handle DB2-specific errors
            System.Diagnostics.Debug.WriteLine($"DB2 Error: {ex.Message}");
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
    /// Populates the grid with results from a DB2 database query (synchronous version)
    /// </summary>
    /// <param name="connectionString">DB2 connection string</param>
    /// <param name="db2Query">DB2 query to execute</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool PopulateFromDb2QuerySync(string connectionString, string db2Query)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(db2Query))
            throw new ArgumentException("DB2 query cannot be null or empty", nameof(db2Query));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            using var connection = new DB2Connection(connectionString);
            connection.Open();

            using var command = new DB2Command(db2Query, connection);
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
        catch (DB2Exception ex)
        {
            // Log or handle DB2-specific errors
            System.Diagnostics.Debug.WriteLine($"DB2 Error: {ex.Message}");
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
    /// Populates the grid with results from a DB2 database query with detailed result information
    /// </summary>
    /// <param name="connectionString">DB2 connection string</param>
    /// <param name="db2Query">DB2 query to execute</param>
    /// <returns>Db2QueryResult with success status, error message, and row count</returns>
    public async Task<Db2QueryResult> PopulateFromDb2QueryAsync(string connectionString, string db2Query)
    {
        var result = new Db2QueryResult();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            result.Success = false;
            result.ErrorMessage = "Connection string cannot be null or empty";
            return result;
        }

        if (string.IsNullOrWhiteSpace(db2Query))
        {
            result.Success = false;
            result.ErrorMessage = "DB2 query cannot be null or empty";
            return result;
        }

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            await using var connection = new DB2Connection(connectionString);
            await connection.OpenAsync();

            await using var command = new DB2Command(db2Query, connection);
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
        catch (DB2Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"DB2 Error: {ex.Message}";
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
