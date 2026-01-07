using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Event Handlers

    private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        // Your logic here

        if (e.KeyModifiers == KeyModifiers.Control)
        {
            // We are scrolling Horizontally

            double d = TheHorizontalScrollBar.Value - (e.Delta.Y * _scrollMultiplier);
            if (d < 0)
            {
                d = 0;
            }
            else if (d > TheHorizontalScrollBar.Maximum)
            {
                d = TheHorizontalScrollBar.Maximum;
            }

            int maxposition = 0;

            for (int i = 0; i < _colWidths.Length; i++)
            {
                maxposition += _colWidths[i];
            }

            double delta = (maxposition / 100) * d;

            _gridXShift = (int)delta;

            TheHorizontalScrollBar.Value = d;

            ReRender();

        }
        else
        {

            if (e.KeyModifiers == KeyModifiers.Alt)
            {
                // Lets Scroll on the grid Font Size

                if (e.Delta.Y > 0)
                {
                    // Increase Font Size
                    GridFontSize += 1;
                    GridHeaderFontSize += 1;
                    GridTitleFontSize += 1;
                }
                else
                {
                    // Decrease Font Size
                    GridFontSize += -1;
                    GridHeaderFontSize += -1;
                    GridTitleFontSize += -1;
                }

                ReRender();
            }
            else
            {
                // We are scrolling Vertically

                double d = TheVerticleScrollBar.Value - (e.Delta.Y * _scrollMultiplier);

                if (d < 0)
                {
                    d = 0;
                }
                else if (d > TheVerticleScrollBar.Maximum)
                {
                    d = TheVerticleScrollBar.Maximum;
                }

                TheVerticleScrollBar.Value = d;

                if (_items.Count > 0)
                {
                    _gridYShift = (int)d;
                    ReRender();
                }
            }

        }

        //this.ReRender();

        e.Handled = true; // Mark the event as handled to prevent it from bubbling up
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        // Get the current pointer position relative to the UserControl
        Point position = e.GetPosition(this);

        _lastPosition = position;

        if (_inDesignMode)
        {
            _curMouseX = (int)position.X;
            _curMouseY = (int)position.Y;
        }
        else
        {
            _curMouseX = (int)position.X - (int)TheCanvas.Bounds.Left;
            _curMouseY = (int)position.Y - (int)TheCanvas.Bounds.Top;
        }

        RecalcItemUnderMouse();

        if (_showCrossHairs && TheItemUnderTheMouseLast.rowID != TheItemUnderTheMouse.rowID)
        {
            //TheItemUnderTheMouseLast = null;
            ReRender();
        }

        this.TheItemUnderTheMouseLast.rowID = this.TheItemUnderTheMouse.rowID;
        this.TheItemUnderTheMouseLast.colID = this.TheItemUnderTheMouse.colID;
        this.TheItemUnderTheMouseLast.cellContent = this.TheItemUnderTheMouse.cellContent;
        this.TheItemUnderTheMouseLast.ItemUnderMouse = this.TheItemUnderTheMouse.ItemUnderMouse;

        e.Handled = true; // Mark the event as handled to prevent it from bubbling up

        //// Get the current pointer position relative to the UserControl
        //Point position = e.GetPosition(this);

        ////this.CurMouseX = (int)position.X;
        //this.CurMouseX = (int)position.X - (int)TheCanvas.Bounds.Left;

        //this.CurMouseY = (int)position.Y - (int)TheCanvas.Bounds.Top;

        //for (int i = 0; i < this.TheLines.Count; i++)
        //{

        //    TCCLineMetrics tccm = this.TheLines[i];

        //    if (this.CurMouseX >= tccm.LineX &&
        //        this.CurMouseX <= tccm.LineX + tccm.LineW &&
        //        this.CurMouseY >= tccm.LineY &&
        //        this.CurMouseY <= tccm.LineH)
        //    {
        //        // we are within a Line
        //        int DateOffset = (int)((this.CurMouseX - this.DaySize) / this.DaySize);

        //        TCCDateHovered tccd =
        //            new TCCDateHovered("Date Hovered", this.BaseDate.AddDays(DateOffset), i);

        //        DateHovered?.Invoke(this, tccd);

        //    }

        //}

        //if (this.DisplayCrossHairs)
        //{
        //    this.ReRender();
        //}

    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        // Get the current pointer position relative to the UserControl
        //Point position = e.GetPosition(this);
        // Do something with this 411

        if (e.GetCurrentPoint((Visual?)sender).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            // Right mouse button was pressed
            // Handle the right button click here
            // Just bail here and pass it off to the context menu

            return;
        }

        // We have a rowclicked event so fire it
        if (TheItemUnderTheMouse.ItemUnderMouse != null)
        {
            GridItemClick?.Invoke(this, TheItemUnderTheMouse);
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            // We are CTRL clicking on items so multi select
            if (_selecteditems.Contains(TheItemUnderTheMouse.ItemUnderMouse))
            {
                // we are unselecting an item
                _selecteditems.Remove(TheItemUnderTheMouse.ItemUnderMouse);
            }
            else
            {
                // we are adding the item to the selection list
                _selecteditems.Add(TheItemUnderTheMouse.ItemUnderMouse);

            }
        }
        else
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                // here we want to see if we are selecting a range of items
                if (_selecteditems.Count > 0)
                {
                    int startrow = -1;
                    int endrow = -1;

                    foreach (AFileEntry af in _items)
                    {
                        if (af == _selecteditems[0])
                        {
                            startrow = _items.IndexOf(af);
                        }

                        if (af == TheItemUnderTheMouse.ItemUnderMouse)
                        {
                            endrow = _items.IndexOf(af);
                        }
                    }

                    if (startrow > endrow)
                    {
                        // we need to swap the start and end rows
                        int temp = startrow;
                        startrow = endrow;
                        endrow = temp;
                    }

                    _selecteditems.Clear();

                    for (int i = startrow; i <= endrow; i++)
                    {
                        _selecteditems.Add(_items[i]);
                    }

                }
                else
                {
                    // we are SHIFT clicking but nothing has been selected yet
                    _selecteditems.Clear();
                    _selecteditems.Add(TheItemUnderTheMouse.ItemUnderMouse);

                }
            }
            else
            {
                // we are not CTRL clicking so single select
                _selecteditems.Clear();
                _selecteditems.Add(TheItemUnderTheMouse.ItemUnderMouse);
            }

            // // we are not CTRL clicking so single select
            // _selecteditems.Clear();
            // _selecteditems.Add(TheItemUnderTheMouse.ItemUnderMouse);

        }
        // Look to handle double click here

        if (e.ClickCount == 1)
        {
            _clickCounter++;
            if (_clickCounter == 2)
            {
                OnDoubleClick(sender, e);
                _clickCounter = 0;
            }
            else
            {
                _doubleClickTimer.Start();
            }
        }
        else
        {
            OnDoubleClick(sender, e);
            _clickCounter = 0;
        }

    }

    private void OnDoubleClick(object? sender, PointerPressedEventArgs e)
    {
        // Handle double-click event here

        if (TheItemUnderTheMouse.ItemUnderMouse != null)
        {
            GridItemDoubleClick?.Invoke(this, TheItemUnderTheMouse);
        }

        //Console.WriteLine("Double-click detected");
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        // Get the current pointer position relative to the UserControl
        //Point position = e.GetPosition(this);
        // Do something with this 411

    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        //this.ShowCrossHairs = true;

        _mouseInControl = true;
        ReRender();

        e.Handled = true;
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        //this.ShowCrossHairs = false;

        //this.ReRender();

        _mouseInControl = false;
        _curMouseX = -1;
        _curMouseY = -1;
        ReRender();

        e.Handled = true;
    }

    private void TheVerticleScrollBar_Scroll(object? sender, ScrollEventArgs e)
    {
        if (_updatingScrollbars) return; // Ignore programmatic updates

        _gridYShift = 0;

        if (_items.Count > 0)
        {
            _gridYShift = (int)e.NewValue;

            // Set flag to prevent ReRender from updating scrollbar back
            _isHandlingScrollEvent = true;
            ReRender();
            _isHandlingScrollEvent = false;
        }

        //throw new NotImplementedException();
    }

    private void TheHorizontalScrollBar_scroll(object? sender, ScrollEventArgs e)
    {
        if (_updatingScrollbars) return; // Ignore programmatic updates

        if (_items.Count > 0)
        {
            int maxposition = 0;

            for (int i = 0; i < _colWidths.Length; i++)
            {
                maxposition += _colWidths[i];
            }

            double delta = (maxposition / 100) * e.NewValue;

            _gridXShift = (int)delta;

            // Set flag to prevent ReRender from updating scrollbar back
            _isHandlingScrollEvent = true;
            ReRender();
            _isHandlingScrollEvent = false;
        }

        //throw new NotImplementedException();
    }

    private void DoubleClickTimer_Tick(object? sender, EventArgs e)
    {
        _clickCounter = 0;
        _doubleClickTimer.Stop();
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Re-render when the canvas size changes (window resize)
        if (!_suspendRendering)
        {
            ReRender();
        }
    }

    #endregion

    #region Context Menu Handlers

    private void Option1_Click(object sender, RoutedEventArgs e)
    {
        this.GridFontSize += 1;
        this.GridTitleFontSize += 1;
        this.GridHeaderFontSize += 1;



    }

    private void Option2_Click(object sender, RoutedEventArgs e)
    {
        // Implement the action for Option 2

        this.GridFontSize += -1;
        this.GridTitleFontSize += -1;
        this.GridHeaderFontSize += -1;

    }

    private async void ExportToExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("ExportToExcel_Click: Starting export...");

            // Export the grid to Excel with a file dialog
            var filePath = await ExportToExcelWithDialog();

            if (filePath != null)
            {
                System.Diagnostics.Debug.WriteLine($"Excel file saved to: {filePath}");

                // Verify file was created
                if (System.IO.File.Exists(filePath))
                {
                    var fileInfo = new System.IO.FileInfo(filePath);
                    System.Diagnostics.Debug.WriteLine($"File verified. Size: {fileInfo.Length} bytes");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: File path returned but file does not exist!");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Excel export cancelled or failed");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ExportToExcel_Click: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    #endregion
}
