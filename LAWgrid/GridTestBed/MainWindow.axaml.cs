using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GridTestBed;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        cmdTest1.Click += CmdTest1OnClick;
        cmdTest2.Click += CmdTest2OnClick;
        cmdTest3.Click += CmdTest3OnClick;
        cmdTest4.Click += CmdTest4OnClick;
        cmdTest5.Click += CmdTest5OnClick;
        cmdTest6.Click += CmdTest6OnClick;
        cmdTest7.Click += CmdTest7OnClick;
        cmdTest8.Click += CmdTest8OnClick;

        TheGridInTest.TestPopulate();
    }

    private async void CmdTest4OnClick(object? sender, RoutedEventArgs e)
    {
        // Create column headers
        var headers = new List<string>
        {
            "Employee ID",
            "Name",
            "Department",
            "Position",
            "Salary",
            "Active",
            "Hire Date"
        };

        // Create sample data rows
        var data = new List<List<string>>
        {
            new List<string> { "001", "John Smith", "Engineering", "Software Engineer", "$95,000", "TRUE", "2020-01-15" },
            new List<string> { "002", "Jane Doe", "Marketing", "Marketing Manager", "$105,000", "TRUE", "2019-06-20" },
            new List<string> { "003", "Bob Johnson", "Sales", "Sales Representative", "$75,000", "TRUE", "2021-03-10" },
            new List<string> { "004", "Alice Williams", "Engineering", "Senior Developer", "$125,000", "TRUE", "2018-09-05" },
            new List<string> { "005", "Charlie Brown", "HR", "HR Specialist", "$70,000", "FALSE", "2022-11-18" },
            new List<string> { "006", "Diana Prince", "Engineering", "DevOps Engineer", "$110,000", "TRUE", "2020-07-22" },
            new List<string> { "007", "Eve Davis", "Finance", "Financial Analyst", "$85,000", "TRUE", "2021-01-08" },
            new List<string> { "008", "Frank Miller", "Sales", "Sales Director", "$135,000", "TRUE", "2017-04-12" },
            new List<string> { "009", "Grace Lee", "Marketing", "Content Strategist", "$80,000", "FALSE", "2023-02-14" },
            new List<string> { "010", "Henry Taylor", "Engineering", "QA Engineer", "$90,000", "TRUE", "2020-10-30" }
        };

        // Use the async version of ListPopulate
        var result = await TheGridInTest.ListPopulateAsync(headers, data);

        // Log the result to the debug console
        if (result.Success)
        {
            Debug.WriteLine($"ListPopulate succeeded: Loaded {result.RowCount} rows with {headers.Count} columns");
        }
        else
        {
            Debug.WriteLine($"ListPopulate failed: {result.ErrorMessage}");
        }
    }

    private void CmdTest5OnClick(object? sender, RoutedEventArgs e)
    {
        // Create a sample DataTable
        DataTable dt = new DataTable("SampleData");

        // Add columns
        dt.Columns.Add("EmployeeID", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Department", typeof(string));
        dt.Columns.Add("Position", typeof(string));
        dt.Columns.Add("Salary", typeof(decimal));
        dt.Columns.Add("Active", typeof(bool));
        dt.Columns.Add("HireDate", typeof(DateTime));

        // Add sample data rows
        dt.Rows.Add(1, "John Smith", "Engineering", "Software Engineer", 95000, true, new DateTime(2020, 1, 15));
        dt.Rows.Add(2, "Jane Doe", "Marketing", "Marketing Manager", 105000, true, new DateTime(2019, 6, 20));
        dt.Rows.Add(3, "Bob Johnson", "Sales", "Sales Representative", 75000, true, new DateTime(2021, 3, 10));
        dt.Rows.Add(4, "Alice Williams", "Engineering", "Senior Developer", 125000, true, new DateTime(2018, 9, 5));
        dt.Rows.Add(5, "Charlie Brown", "HR", "HR Specialist", 70000, false, new DateTime(2022, 11, 18));
        dt.Rows.Add(6, "Diana Prince", "Engineering", "DevOps Engineer", 110000, true, new DateTime(2020, 7, 22));
        dt.Rows.Add(7, "Eve Davis", "Finance", "Financial Analyst", 85000, true, new DateTime(2021, 1, 8));
        dt.Rows.Add(8, "Frank Miller", "Sales", "Sales Director", 135000, true, new DateTime(2017, 4, 12));
        dt.Rows.Add(9, "Grace Lee", "Marketing", "Content Strategist", 80000, false, new DateTime(2023, 2, 14));
        dt.Rows.Add(10, "Henry Taylor", "Engineering", "QA Engineer", 90000, true, new DateTime(2020, 10, 30));

        // Populate the grid from the DataTable
        bool success = TheGridInTest.PopulateFromDataTable(dt);

        // Log the result to the debug console
        if (success)
        {
            Debug.WriteLine($"PopulateFromDataTable succeeded: Loaded {dt.Rows.Count} rows with {dt.Columns.Count} columns");
        }
        else
        {
            Debug.WriteLine("PopulateFromDataTable failed");
        }
    }

    private void CmdTest3OnClick(object? sender, RoutedEventArgs e)
    {
        TheGridInTest.TestPopulate();
    }

    private async void CmdTest2OnClick(object? sender, RoutedEventArgs e)
    {
        string ConnectionString = "Server=luisbhds.database.windows.net;Database=LUISBHDS_TEST;User Id={MyUserName};Password={MyPasswordHere}";
        
        string Query = "SELECT top 100 * FROM dbo.MEMBERMAIN";

        var result = await TheGridInTest.PopulateFromSqlQueryAsync(ConnectionString, Query);

        // Log the result to the debug console
        if (result.Success)
        {
            Debug.WriteLine($"Async SQL Query succeeded: Loaded {result.RowCount} rows");
        }
        else
        {
            Debug.WriteLine($"Async SQL Query failed: {result.ErrorMessage}");
        }
    }

    private void CmdTest1OnClick(object? sender, RoutedEventArgs e)
    {
        string ConnectionString = "Server=luisbhds.database.windows.net;Database=LUISBHDS_TEST;User Id={MyUserName};Password={MyPasswordHere}";

        string Query = "SELECT top 100 * FROM dbo.MEMBERMAIN";

        TheGridInTest.RenderBooleansAsImages = false;
        TheGridInTest.PopulateFromSqlQuerySync(ConnectionString, Query);


    }

    private void CmdTest6OnClick(object? sender, RoutedEventArgs e)
    {
        // Toggle the GreenBarMode
        TheGridInTest.ToggleGreenBarMode();

        // Log the current state to debug console
        Debug.WriteLine($"GreenBarMode is now: {TheGridInTest.GreenBarMode}");
    }

    private void CmdTest7OnClick(object? sender, RoutedEventArgs e)
    {
        // Save the grid to Desktop
        string? filePath = TheGridInTest.SaveGridToDesktop();

        // Log the result to debug console
        if (filePath != null)
        {
            Debug.WriteLine($"Grid successfully saved to: {filePath}");
            Debug.WriteLine("You can now open this file with any image viewer or paste it into documents.");
        }
        else
        {
            Debug.WriteLine("Failed to save grid to Desktop");
        }
    }

    private void CmdTest8OnClick(object? sender, RoutedEventArgs e)
    {
        // Create headers: MONTH1, MONTH2, ... MONTH24
        string[] headers = new string[24];
        for (int i = 0; i < 24; i++)
        {
            headers[i] = "MONTH" + (i + 1).ToString();
        }

        // Create 24x24 array of '0.00'
        string[,] data = new string[24, 24];
        for (int row = 0; row < 24; row++)
        {
            for (int col = 0; col < 24; col++)
            {
                data[row, col] = "0.00";
            }
        }

        // Populate the grid
        TheGridInTest.PopulateFromArrays(headers, data);

        // Log to debug console
        Debug.WriteLine("Grid populated with 24x24 array of '0.00' values");
    }
}
