using System;
using System.Collections.Generic;
using System.Dynamic;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region List Population Methods

    /// <summary>
    /// Populates the grid from a list of column headers and a list of data rows
    /// </summary>
    /// <param name="columnHeaders">List of column header names</param>
    /// <param name="actualData">List of rows, where each row is a List of string values</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool ListPopulate(List<string> columnHeaders, List<List<string>> actualData)
    {
        if (columnHeaders == null)
            throw new ArgumentNullException(nameof(columnHeaders), "Column headers cannot be null");

        if (actualData == null)
            throw new ArgumentNullException(nameof(actualData), "Actual data cannot be null");

        if (columnHeaders.Count == 0)
            throw new ArgumentException("Column headers list cannot be empty", nameof(columnHeaders));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            // Convert the data to ExpandoObjects for rendering
            foreach (var row in actualData)
            {
                var expando = new ExpandoObject() as IDictionary<string, object>;

                // Ensure we don't have more values than headers
                int columnCount = Math.Min(columnHeaders.Count, row.Count);

                for (int i = 0; i < columnCount; i++)
                {
                    string columnName = columnHeaders[i];
                    string value = row[i] ?? string.Empty;
                    expando[columnName] = value;
                }

                // If row has fewer values than headers, fill with empty strings
                for (int i = row.Count; i < columnHeaders.Count; i++)
                {
                    string columnName = columnHeaders[i];
                    expando[columnName] = string.Empty;
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ListPopulate: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Populates the grid from a list of column headers and a list of data rows (async version)
    /// </summary>
    /// <param name="columnHeaders">List of column header names</param>
    /// <param name="actualData">List of rows, where each row is a List of string values</param>
    /// <returns>ListPopulateResult with success status, error message, and row count</returns>
    public async System.Threading.Tasks.Task<ListPopulateResult> ListPopulateAsync(List<string> columnHeaders, List<List<string>> actualData)
    {
        var result = new ListPopulateResult();

        if (columnHeaders == null)
        {
            result.Success = false;
            result.ErrorMessage = "Column headers cannot be null";
            return result;
        }

        if (actualData == null)
        {
            result.Success = false;
            result.ErrorMessage = "Actual data cannot be null";
            return result;
        }

        if (columnHeaders.Count == 0)
        {
            result.Success = false;
            result.ErrorMessage = "Column headers list cannot be empty";
            return result;
        }

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            // Convert the data to ExpandoObjects for rendering
            int rowCount = 0;
            foreach (var row in actualData)
            {
                var expando = new ExpandoObject() as IDictionary<string, object>;

                // Ensure we don't have more values than headers
                int columnCount = Math.Min(columnHeaders.Count, row.Count);

                for (int i = 0; i < columnCount; i++)
                {
                    string columnName = columnHeaders[i];
                    string value = row[i] ?? string.Empty;
                    expando[columnName] = value;
                }

                // If row has fewer values than headers, fill with empty strings
                for (int i = row.Count; i < columnHeaders.Count; i++)
                {
                    string columnName = columnHeaders[i];
                    expando[columnName] = string.Empty;
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
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Error in ListPopulateAsync: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
            return result;
        }
    }

    #endregion
}

/// <summary>
/// Result class for ListPopulateAsync method
/// </summary>
public class ListPopulateResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int RowCount { get; set; }
}
