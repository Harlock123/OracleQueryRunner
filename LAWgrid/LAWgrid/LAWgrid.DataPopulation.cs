using System;
using System.Collections.Generic;
using System.Data;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Data Population Methods

    public void TestPopulate()
    {
        PopulateTestData();
    }

    /// <summary>
    /// Populates the grid with column headers and data from 2D string arrays
    /// </summary>
    /// <param name="columnHeaders">2D array where each row contains [PropertyName, DisplayHeader]</param>
    /// <param name="dataValues">2D array where each row represents a data row, and columns correspond to the headers</param>
    public void PopulateFromArrays(string[,] columnHeaders, string[,] dataValues)
    {
        if (columnHeaders == null || columnHeaders.GetLength(1) < 1)
            throw new ArgumentException("Column headers array must have at least 1 column", nameof(columnHeaders));

        if (dataValues == null)
            throw new ArgumentException("Data values array cannot be null", nameof(dataValues));

        int headerCount = columnHeaders.GetLength(0);
        int columnCount = dataValues.GetLength(1);

        if (columnCount != headerCount)
            throw new ArgumentException($"Number of data columns ({columnCount}) must match number of headers ({headerCount})");

        // Clear existing items
        _items.Clear();
        _selecteditems.Clear();

        // Create dynamic objects from the data
        int rowCount = dataValues.GetLength(0);

        for (int row = 0; row < rowCount; row++)
        {
            var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

            for (int col = 0; col < columnCount; col++)
            {
                string propertyName = columnHeaders[col, 0];
                string value = dataValues[row, col] ?? string.Empty;
                expando[propertyName] = value;
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
    }

    /// <summary>
    /// Populates the grid with column headers (as simple strings) and data from 2D string arrays
    /// Property names will be the same as the display headers
    /// </summary>
    /// <param name="columnHeaders">1D array of column header names</param>
    /// <param name="dataValues">2D array where each row represents a data row, and columns correspond to the headers</param>
    public void PopulateFromArrays(string[] columnHeaders, string[,] dataValues)
    {
        if (columnHeaders == null || columnHeaders.Length < 1)
            throw new ArgumentException("Column headers array must have at least 1 element", nameof(columnHeaders));

        if (dataValues == null)
            throw new ArgumentException("Data values array cannot be null", nameof(dataValues));

        int columnCount = dataValues.GetLength(1);

        if (columnCount != columnHeaders.Length)
            throw new ArgumentException($"Number of data columns ({columnCount}) must match number of headers ({columnHeaders.Length})");

        // Clear existing items
        _items.Clear();
        _selecteditems.Clear();

        // Create dynamic objects from the data
        int rowCount = dataValues.GetLength(0);

        for (int row = 0; row < rowCount; row++)
        {
            var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

            for (int col = 0; col < columnCount; col++)
            {
                string propertyName = columnHeaders[col];
                string value = dataValues[row, col] ?? string.Empty;
                expando[propertyName] = value;
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
    }

    /// <summary>
    /// Populates the grid from a .NET DataTable object
    /// </summary>
    /// <param name="dataTable">The DataTable containing the data to display</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool PopulateFromDataTable(DataTable dataTable)
    {
        if (dataTable == null)
            throw new ArgumentNullException(nameof(dataTable), "DataTable cannot be null");

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            // Get column names from DataTable
            var columnNames = new List<string>();
            foreach (DataColumn column in dataTable.Columns)
            {
                columnNames.Add(column.ColumnName);
            }

            // Read all rows
            foreach (DataRow row in dataTable.Rows)
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                foreach (var columnName in columnNames)
                {
                    object value = row[columnName];

                    // Handle DBNull and convert to string for display
                    if (value == DBNull.Value || value == null)
                    {
                        expando[columnName] = string.Empty;
                    }
                    else
                    {
                        expando[columnName] = value.ToString() ?? string.Empty;
                    }
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
            // Log or handle errors
            System.Diagnostics.Debug.WriteLine($"Error populating from DataTable: {ex.Message}");
            return false;
        }
    }

    public void ClearTestPopulate()
    {
        Dispatcher.UIThread.Post(() =>
        {
            TheCanvas.Children.Clear();
            _items.Clear();
            _gridXShift = 0;
            _gridYShift = 0;
            ReRender();
        });
    }

    private void PopulateTestData()
    {
        TheCanvas.Children.Clear();
        _items.Clear();

        for (int i = 0; i < 20; i++)
        {
            TestStuff tt = new TestStuff();

            tt.Name = "Lonnie Watson";
            tt.Status = "Active";
            tt.AssignedTo = "SlatriBartFast";
            tt.Priority = "High";
            //tt.DueDate = DateTime.Now.AddDays(5).ToShortDateString();
            tt.Description = "This is a test of the emergency broadcast system\n.This is only a test.";


            tt.Id = i;
            tt.DueDate = DateTime.Now.AddDays(i).ToShortDateString();
            tt.AssignedDate = DateTime.Now.AddDays(i).ToShortDateString();
            tt.Name = "Lonnie Watson " + i.ToString();

            tt.CompletedAssignedBy = "Name Of Person " + i.ToString();

            _items.Add(tt);
        }

        // Reset scroll positions and render on the UI thread
        Dispatcher.UIThread.Post(() =>
        {
            _gridXShift = 0;
            _gridYShift = 0;
            ReRender();
        });
    }

    #endregion
}
