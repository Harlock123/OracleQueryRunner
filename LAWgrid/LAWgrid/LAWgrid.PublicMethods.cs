using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Public Utility Methods

    public void SetGridSize(int width, int height)
    {
        // Note: When using Stretch alignment in XAML, Canvas size is determined by container
        // This method allows programmatic sizing but may be overridden by layout system
        TheCanvas.Width = width;
        TheCanvas.Height = height;
        Width = width + 12;
        Height = height + 12;
        ReRender();
    }

    public string GetFirstSelectedFolder()
    {
        string result = "";

        foreach(AFileEntry item in SelectedItems)
        {
            //var item = i.ItemUnderMouse as AFileEntry;

            if ( item.Typ)
            {
                result = item.Name;
                break;
            }
        }

        return result;
    }

    public List<AFileEntry> GetListOfSelectedFiles()
    {
        List<AFileEntry> result = new List<AFileEntry>();

        foreach (AFileEntry item in SelectedItems)
        {
            //var item = i.ItemUnderMouse as AFileEntry;

            if (!item.Typ)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public void SelectAllFilesOnly()
    {
        SelectedItems.Clear();

        foreach (AFileEntry item in Items)
        {
            if (!item.Typ)
            {
                SelectedItems.Add(item);
            }
        }

        ReRender();
    }

    public void SelectAllFoldersOnly()
    {
        SelectedItems.Clear();

        foreach (AFileEntry item in Items)
        {
            if (item.Typ)
            {
                SelectedItems.Add(item);
            }
        }

        ReRender();
    }

    /// <summary>
    /// Toggles the GreenBarMode on or off (alternating row colors)
    /// </summary>
    public void ToggleGreenBarMode()
    {
        GreenBarMode = !GreenBarMode;
    }

    /// <summary>
    /// Saves the grid to the Desktop as a PNG file
    /// </summary>
    /// <returns>The full path to the saved file, or null if failed</returns>
    public string? SaveGridToDesktop()
    {
        try
        {
            // Get the size of the control
            var pixelSize = new PixelSize((int)Bounds.Width, (int)Bounds.Height);

            if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Invalid grid size for capture");
                return null;
            }

            // Create a RenderTargetBitmap with the control's size
            var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(96, 96));

            // Render the control to the bitmap
            renderTarget.Render(this);

            // Save to Desktop
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileName = $"grid_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var fullPath = Path.Combine(desktopPath, fileName);

            renderTarget.Save(fullPath);

            System.Diagnostics.Debug.WriteLine($"Grid saved to Desktop: {fullPath}");
            return fullPath;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving grid to Desktop: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Captures the current grid visual as a PNG and copies it to the clipboard
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> CopyGridToClipboardAsync()
    {
        try
        {
            // Get the clipboard from the TopLevel (works cross-platform)
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.Clipboard == null)
            {
                System.Diagnostics.Debug.WriteLine("Clipboard not available");
                return false;
            }

            // Get the size of the control
            var pixelSize = new PixelSize((int)Bounds.Width, (int)Bounds.Height);

            if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Invalid grid size for capture");
                return false;
            }

            // Create a RenderTargetBitmap with the control's size
            var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(96, 96));

            // Render the control to the bitmap
            renderTarget.Render(this);

            // Save as a temporary file
            var tempPath = Path.Combine(Path.GetTempPath(), $"grid_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            renderTarget.Save(tempPath);

            // Clear clipboard first
            await topLevel.Clipboard.ClearAsync();

            // Create a DataObject and set the file
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Files, new[] { tempPath });

            // Set the clipboard
            await topLevel.Clipboard.SetDataObjectAsync(dataObject);

            System.Diagnostics.Debug.WriteLine($"Grid image saved and file path copied to clipboard: {tempPath} ({pixelSize.Width}x{pixelSize.Height})");
            System.Diagnostics.Debug.WriteLine($"You can paste this file into applications that accept file drops.");

            return true;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error copying grid to clipboard: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Saves the current grid visual as a PNG file
    /// </summary>
    /// <param name="filePath">The file path where to save the PNG</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool SaveGridAsPng(string filePath)
    {
        try
        {
            // Get the size of the control
            var pixelSize = new PixelSize((int)Bounds.Width, (int)Bounds.Height);

            // Create a RenderTargetBitmap with the control's size
            var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(96, 96));

            // Render the control to the bitmap
            renderTarget.Render(this);

            // Save the bitmap to file
            renderTarget.Save(filePath);

            System.Diagnostics.Debug.WriteLine($"Grid saved to: {filePath} ({pixelSize.Width}x{pixelSize.Height})");
            return true;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving grid as PNG: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Saves the current grid visual as a JPEG file
    /// </summary>
    /// <param name="filePath">The file path where to save the JPEG</param>
    /// <param name="quality">JPEG quality (0-100, default 90)</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool SaveGridAsJpg(string filePath, int quality = 90)
    {
        try
        {
            // Get the size of the control
            var pixelSize = new PixelSize((int)Bounds.Width, (int)Bounds.Height);

            if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Invalid grid size for capture");
                return false;
            }

            // Create a RenderTargetBitmap with the control's size
            var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(96, 96));

            // Render the control to the bitmap
            renderTarget.Render(this);

            // Save to a temporary PNG first
            using (var pngStream = new MemoryStream())
            {
                renderTarget.Save(pngStream);
                pngStream.Position = 0;

                // Load into SkiaSharp for conversion
                using (var skBitmap = SKBitmap.Decode(pngStream))
                {
                    if (skBitmap == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to decode bitmap");
                        return false;
                    }

                    // Encode as JPEG with specified quality
                    using (var image = SKImage.FromBitmap(skBitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Jpeg, quality))
                    using (var fileStream = File.OpenWrite(filePath))
                    {
                        data.SaveTo(fileStream);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Grid saved as JPEG to: {filePath} ({pixelSize.Width}x{pixelSize.Height}, quality: {quality})");
            return true;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving grid as JPEG: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Saves the current grid visual as a BMP file
    /// </summary>
    /// <param name="filePath">The file path where to save the BMP</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool SaveGridAsBmp(string filePath)
    {
        try
        {
            // Get the size of the control
            var pixelSize = new PixelSize((int)Bounds.Width, (int)Bounds.Height);

            if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Invalid grid size for capture");
                return false;
            }

            // Create a RenderTargetBitmap with the control's size
            var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(96, 96));

            // Render the control to the bitmap
            renderTarget.Render(this);

            // Save to a temporary PNG first
            using (var pngStream = new MemoryStream())
            {
                renderTarget.Save(pngStream);
                pngStream.Position = 0;

                // Load into SkiaSharp for conversion
                using (var skBitmap = SKBitmap.Decode(pngStream))
                {
                    if (skBitmap == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to decode bitmap");
                        return false;
                    }

                    // Encode as BMP
                    using (var image = SKImage.FromBitmap(skBitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Bmp, 100))
                    using (var fileStream = File.OpenWrite(filePath))
                    {
                        data.SaveTo(fileStream);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Grid saved as BMP to: {filePath} ({pixelSize.Width}x{pixelSize.Height})");
            return true;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving grid as BMP: {ex.Message}");
            return false;
        }
    }

    #endregion
}
