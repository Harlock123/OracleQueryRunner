using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform.Storage;
using ClosedXML.Excel;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Excel Export Methods

    /// <summary>
    /// Exports the grid contents to an Excel file with formatting that matches the grid
    /// </summary>
    /// <param name="filePath">The full path where the Excel file should be saved</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool ExportToExcel(string filePath)
    {
        try
        {
            // Check if we have any data
            if (_items.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No data to export - _items is empty");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"Exporting {_items.Count} items to Excel");

            // Get column names from the first item (handle both ExpandoObject and regular objects)
            List<string> columnNames;
            var firstItem = _items[0];

            if (firstItem is IDictionary<string, object> expandoDict)
            {
                // Handle ExpandoObject (from database queries)
                columnNames = expandoDict.Keys.ToList();
                System.Diagnostics.Debug.WriteLine($"Found ExpandoObject with {columnNames.Count} columns: {string.Join(", ", columnNames)}");
            }
            else
            {
                // Handle regular objects (like TestStuff) using reflection
                var properties = firstItem.GetType().GetProperties();
                columnNames = properties.Select(p => p.Name).ToList();
                System.Diagnostics.Debug.WriteLine($"Found {firstItem.GetType().Name} with {columnNames.Count} properties: {string.Join(", ", columnNames)}");
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("EXCELEXPORT");

            int currentRow = 1;

            // Write title if it exists
            if (!string.IsNullOrWhiteSpace(_gridTitle))
            {
                var titleRange = worksheet.Range(currentRow, 1, currentRow, columnNames.Count);
                titleRange.Merge();
                titleRange.Value = _gridTitle;
                titleRange.Style.Font.FontSize = _gridTitleFontSize;
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Apply title background color
                ApplyBrushToClosedXMLCell(titleRange, _gridTitleBackground, _gridTitleBrush);

                currentRow++;
            }

            // Write column headers
            for (int col = 0; col < columnNames.Count; col++)
            {
                var cell = worksheet.Cell(currentRow, col + 1);
                cell.Value = columnNames[col];
                cell.Style.Font.FontSize = _gridheaderFontSize;
                cell.Style.Font.Bold = true;

                // Apply header formatting
                ApplyBrushToClosedXMLCell(cell.AsRange(), _gridHeaderBackground, _gridHeaderBrush);

                // Add border
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
            currentRow++;

            // Write data rows
            int dataRowIndex = 0;
            foreach (var item in _items)
            {
                for (int col = 0; col < columnNames.Count; col++)
                {
                    var cell = worksheet.Cell(currentRow, col + 1);
                    string cellValue = string.Empty;

                    // Get value based on item type
                    if (item is IDictionary<string, object> itemDict)
                    {
                        // ExpandoObject - use dictionary access
                        if (itemDict.TryGetValue(columnNames[col], out var value))
                        {
                            cellValue = value?.ToString() ?? string.Empty;
                        }
                    }
                    else
                    {
                        // Regular object - use reflection
                        var property = item.GetType().GetProperty(columnNames[col]);
                        if (property != null)
                        {
                            var value = property.GetValue(item);
                            cellValue = value?.ToString() ?? string.Empty;
                        }
                    }

                    cell.Value = cellValue;
                    cell.Style.Font.FontSize = _gridFontSize;

                    // Apply cell formatting based on GreenBarMode
                    if (_greenBarMode)
                    {
                        // Alternate row colors
                        var bgBrush = (dataRowIndex % 2 == 0) ? _greenBarColor1 : _greenBarColor2;
                        ApplyBrushToClosedXMLCell(cell.AsRange(), bgBrush, _gridCellContentBrush);
                    }
                    else
                    {
                        ApplyBrushToClosedXMLCell(cell.AsRange(), _gridCellBrush, _gridCellContentBrush);
                    }

                    // Add border
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                currentRow++;
                dataRowIndex++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save the file
            System.Diagnostics.Debug.WriteLine($"Attempting to save Excel file to: {filePath}");
            workbook.SaveAs(filePath);

            System.Diagnostics.Debug.WriteLine($"Excel file saved successfully. File size: {new FileInfo(filePath).Length} bytes");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exporting to Excel: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Shows a save file dialog and exports the grid to Excel
    /// </summary>
    /// <returns>The path to the saved file, or null if cancelled or failed</returns>
    public async Task<string?> ExportToExcelWithDialog()
    {
        try
        {
            // Get the top level window
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
            {
                System.Diagnostics.Debug.WriteLine("Could not get top level window");
                return null;
            }

            // Create default filename with current date
            var defaultFileName = $"EXCELExport_{DateTime.Now:MM-dd-yyyy}.xlsx";

            // Show save file dialog
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export to Excel",
                SuggestedFileName = defaultFileName,
                DefaultExtension = "xlsx",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Excel Files")
                    {
                        Patterns = new[] { "*.xlsx" }
                    }
                }
            });

            if (file != null)
            {
                var filePath = file.Path.LocalPath;

                // Perform the export
                if (ExportToExcel(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ExportToExcelWithDialog: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Helper method to apply Avalonia brush colors to ClosedXML cells
    /// </summary>
    private void ApplyBrushToClosedXMLCell(IXLRange cell, IBrush backgroundBrush, IBrush textBrush)
    {
        try
        {
            // Debug: Log brush types
            System.Diagnostics.Debug.WriteLine($"Applying colors - BG Brush Type: {backgroundBrush?.GetType().Name ?? "null"}, Text Brush Type: {textBrush?.GetType().Name ?? "null"}");

            // Apply background color
            if (backgroundBrush is SolidColorBrush bgSolidBrush)
            {
                var bgColor = bgSolidBrush.Color;
                System.Diagnostics.Debug.WriteLine($"Background Color: A={bgColor.A}, R={bgColor.R}, G={bgColor.G}, B={bgColor.B}");

                var xlColor = XLColor.FromArgb(bgColor.R, bgColor.G, bgColor.B);
                cell.Style.Fill.PatternType = ClosedXML.Excel.XLFillPatternValues.Solid;
                cell.Style.Fill.BackgroundColor = xlColor;
            }
            else if (backgroundBrush is ImmutableSolidColorBrush immutableBgBrush)
            {
                var bgColor = immutableBgBrush.Color;
                System.Diagnostics.Debug.WriteLine($"Immutable Background Color: A={bgColor.A}, R={bgColor.R}, G={bgColor.G}, B={bgColor.B}");

                var xlColor = XLColor.FromArgb(bgColor.R, bgColor.G, bgColor.B);
                cell.Style.Fill.PatternType = ClosedXML.Excel.XLFillPatternValues.Solid;
                cell.Style.Fill.BackgroundColor = xlColor;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Background brush not a SolidColorBrush: {backgroundBrush?.GetType().Name ?? "null"}");
            }

            // Apply text color
            if (textBrush is SolidColorBrush textSolidBrush)
            {
                var textColor = textSolidBrush.Color;
                System.Diagnostics.Debug.WriteLine($"Text Color: A={textColor.A}, R={textColor.R}, G={textColor.G}, B={textColor.B}");

                var xlTextColor = XLColor.FromArgb(textColor.R, textColor.G, textColor.B);
                cell.Style.Font.FontColor = xlTextColor;
            }
            else if (textBrush is ImmutableSolidColorBrush immutableTextBrush)
            {
                var textColor = immutableTextBrush.Color;
                System.Diagnostics.Debug.WriteLine($"Immutable Text Color: A={textColor.A}, R={textColor.R}, G={textColor.G}, B={textColor.B}");

                var xlTextColor = XLColor.FromArgb(textColor.R, textColor.G, textColor.B);
                cell.Style.Font.FontColor = xlTextColor;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Text brush not a SolidColorBrush: {textBrush?.GetType().Name ?? "null"}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying brush to cell: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    #endregion
}
