using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Rendering Methods

    public void ReRender()
    {
        try
        {

            if (TheCanvas != null && !_suspendRendering)
            {
                // the canvas exists so lets render the grid

                // clear the canvas
                TheCanvas.Children.Clear();

                _gridHeaderAndTitleHeight = 0;

                #region Render Title

                if (GridTitle != String.Empty)
                {
                    var formattedText =
                        new FormattedText(GridTitle, CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, GridTitleTypeface, GridTitleFontSize,
                        GridTitleBrush);

                    Rectangle rr = new Rectangle();
                    rr.Width = TheCanvas.Bounds.Width;
                    //rr.Height = formattedText.Height;

                    if (GridTitleHeight < formattedText.Height)
                    {
                        rr.Height = formattedText.Height;
                        _gridTitleHeight = (int)formattedText.Height;
                        _gridHeaderAndTitleHeight = _gridTitleHeight;
                    }
                    else
                    {
                        rr.Height = GridTitleHeight;

                        _gridTitleHeight = (int)formattedText.Height;

                        _gridHeaderAndTitleHeight = _gridTitleHeight;
                    }

                    //this.gridHeaderAndTitleHeight = this.gridTitleHeight;

                    rr.Fill = GridTitleBackground;
                    Canvas.SetLeft(rr, 0);
                    Canvas.SetTop(rr, 0);
                    TheCanvas.Children.Add(rr);

                    TextBlock ttb = new TextBlock();
                    ttb.Text = GridTitle;
                    ttb.FontSize = GridTitleFontSize;
                    ttb.Foreground = GridTitleBrush;
                    ttb.FontFamily = GridTitleTypeface.FontFamily;
                    ttb.FontWeight = GridTitleTypeface.Weight;
                    ttb.FontStyle = GridTitleTypeface.Style;
                    Canvas.SetLeft(ttb, _gridLeftOffset);
                    Canvas.SetTop(ttb, 0);
                    TheCanvas.Children.Add(ttb);

                }

                #endregion

                #region FigureOut Cell Sizes

                _rowHeights = new int[_items.Count];
                _gridRows = _items.Count;
                int row = 0;

                if (_items.Count > 0)
                {
                    List<PropertyInfoModel> schema = GetObjectSchema(_items[0]);

                    _colWidths = new int[schema.Count];
                    _gridCols = schema.Count;
                    int idx = 0;
                    //int tempval = 0;

                    foreach (PropertyInfoModel property in schema)
                    {
                        var formattedText =
                            new FormattedText(property.Name, CultureInfo.CurrentCulture,
                                                       FlowDirection.LeftToRight,
                                                       GridHeaderTypeface,
                                                       GridHeaderFontSize,
                                                       GridTitleBrush);

                        _colWidths[idx] = (int)formattedText.Width + _cellPaddingWidth;

                        idx++;
                    }



                    // we now have column widths, lets see if we need to autosize the cells

                    if (AutosizeCellsToContents)
                    {
                        // we need to autosize the cells

                        var rowidx = 0;

                        foreach (object item in _items)
                        {
                            idx = 0;

                            int tempval = 0;


                            // Get schema to iterate through columns
                            List<PropertyInfoModel> itemSchema = GetObjectSchema(item);

                            foreach (PropertyInfoModel schemaItem in itemSchema)
                            {
                                // Get the value - handle both regular objects and ExpandoObjects
                                object? cellValue = null;
                                if (item is IDictionary<string, object> dictionary)
                                {
                                    // Handle ExpandoObject/dictionary
                                    dictionary.TryGetValue(schemaItem.Name, out cellValue);
                                }
                                else
                                {
                                    // Handle regular object with properties
                                    PropertyInfo? property = item.GetType().GetProperty(schemaItem.Name);
                                    if (property != null)
                                    {
                                        cellValue = property.GetValue(item);
                                    }
                                }

                                string thestringtomeasure = cellValue?.ToString() + "";

                                if (_truncateColumns.Contains(idx))
                                {
                                    if (thestringtomeasure.Length > _truncateColumnLength)
                                    {
                                        thestringtomeasure = thestringtomeasure.Substring(0, _truncateColumnLength ) + "...";
                                    }

                                }

                                var formattedText =
                                    new FormattedText(thestringtomeasure,
                                            CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                            _gridTypeface, _gridFontSize,
                                            _gridCellBrush);
                                if (_colWidths[idx] < formattedText.Width + _cellPaddingHeight)
                                {
                                    _colWidths[idx] = (int)formattedText.Width + _cellPaddingWidth;
                                }

                                if ((int)formattedText.Height + _cellPaddingHeight > tempval)
                                {
                                    tempval = (int)formattedText.Height + _cellPaddingHeight;
                                }

                                idx++;
                            }

                            _rowHeights[rowidx] = tempval;
                            tempval = 0;
                            rowidx++;

                        }
                    }
                }


                #endregion

                #region Render Grid Header

                if (_items.Count > 0)
                {
                    List<PropertyInfoModel> schema = GetObjectSchema(_items[0]);
                    int left = _gridLeftOffset;  // Platform-specific offset
                    int top = _gridTitleHeight;
                    int tempheight = 0;


                    int idx = 0;

                    foreach (PropertyInfoModel property in schema)
                    {
                        var formattedText =
                            new FormattedText(property.Name, CultureInfo.CurrentCulture,
                                                       FlowDirection.LeftToRight,
                                                       GridHeaderTypeface,
                                                       GridHeaderFontSize,
                                                       GridTitleBrush);

                        // see if the height of the text is greater than the current height
                        // gathered for the header physical height on the grid
                        if (tempheight == 0)
                        {
                            tempheight = (int)formattedText.Height;
                        }
                        else
                        {
                            if (tempheight < formattedText.Height)
                            {
                                tempheight = (int)formattedText.Height;
                            }
                        }

                        Rectangle rr1 = new Rectangle();
                        rr1.Width = _colWidths[idx];

                        //this.colWidths[idx] = (int)rr1.Width;

                        rr1.Height = (int)formattedText.Height + 2;
                        rr1.Fill = GridHeaderBackground;
                        //rr1.StrokeThickness = 1;
                        Canvas.SetLeft(rr1, left - _gridXShift);
                        Canvas.SetTop(rr1, top);

                        TheCanvas.Children.Add(rr1);

                        Rectangle rr = new Rectangle();
                        rr.Width = _colWidths[idx];//(int)formattedText.Width + 10;
                        rr.Height = (int)formattedText.Height + 2;
                        rr.Stroke = _gridCellOutline;
                        rr.StrokeThickness = 1;
                        Canvas.SetLeft(rr, left - _gridXShift);
                        Canvas.SetTop(rr, top);

                        TheCanvas.Children.Add(rr);

                        TextBlock ttb = new TextBlock();
                        ttb.Text = property.Name;
                        ttb.FontSize = GridHeaderFontSize;
                        ttb.Foreground = GridHeaderBrush;
                        ttb.FontFamily = GridHeaderTypeface.FontFamily;
                        ttb.FontWeight = GridHeaderTypeface.Weight;
                        ttb.FontStyle = GridHeaderTypeface.Style;
                        Canvas.SetLeft(ttb, left + 2 - _gridXShift);
                        Canvas.SetTop(ttb, top + 1);
                        TheCanvas.Children.Add(ttb);
                        left += _colWidths[idx];

                        idx++;
                    }

                    _gridHeaderAndTitleHeight += tempheight;
                }

                #endregion

                // coming out of here we should have our grids Title and Header Row rendered
                // we should also know at what Y coordinate to start rendering the data rows
                // as it will be the gridHeaderAndTitleHeight

                #region Render Grid Data Rows

                if (_items.Count > 0)
                {
                    int left = _gridLeftOffset;  // Platform-specific offset
                    int top = _gridHeaderAndTitleHeight;

                    int idx = 0;
                    int rowidx = 0;

                    foreach (object item in _items)
                    {

                        // Set default cell background brush based on GreenBarMode
                        IBrush tbb;
                        if (_greenBarMode)
                        {
                            // Alternate between GreenBarColor1 and GreenBarColor2
                            tbb = (rowidx % 2 == 0) ? _greenBarColor1 : _greenBarColor2;
                        }
                        else
                        {
                            tbb = _gridCellBrush;
                        }

                        IBrush tcb = _gridCellContentBrush;

                        if (_selecteditems.Contains(item))
                        {
                            tbb = _gridSelectedItemBrush;
                            tcb = _gridCellHighlightContentBrush;
                        }

                        if (rowidx == TheItemUnderTheMouse.rowID && _mouseInControl)
                        {
                            tbb = _gridCellHighlightBrush;
                            tcb = _gridCellHighlightContentBrush;
                        }

                        idx = 0;
                        left = _gridLeftOffset;

                        if (rowidx < _gridYShift)
                        {
                            rowidx++;
                        }
                        else
                        {
                            string lastCellItemText = "";

                            // Get schema to iterate through columns
                            List<PropertyInfoModel> schema = GetObjectSchema(item);

                            foreach (PropertyInfoModel schemaItem in schema)
                            {
                                // Get the value - handle both regular objects and ExpandoObjects
                                object? cellValue = null;
                                if (item is IDictionary<string, object> dictionary)
                                {
                                    // Handle ExpandoObject/dictionary
                                    dictionary.TryGetValue(schemaItem.Name, out cellValue);
                                }
                                else
                                {
                                    // Handle regular object with properties
                                    PropertyInfo? property = item.GetType().GetProperty(schemaItem.Name);
                                    if (property != null)
                                    {
                                        cellValue = property.GetValue(item);
                                    }
                                }

                                var formattedText =
                                    new FormattedText(cellValue?.ToString() + "",
                                            CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                            _gridTypeface, _gridFontSize,
                                            _gridCellBrush);

                                Rectangle rr = new Rectangle();
                                rr.Width = _colWidths[idx];
                                rr.Height = _rowHeights[rowidx];

                                rr.Fill = tbb;

                                // Do our Grid Clipping Here
                                if ((left - _gridXShift + rr.Width > 0) &&
                                    (top <= TheCanvas.Bounds.Height) &&
                                    (left - _gridXShift <= TheCanvas.Bounds.Width))
                                {

                                    Canvas.SetLeft(rr, left - _gridXShift);
                                    Canvas.SetTop(rr, top);

                                    TheCanvas.Children.Add(rr);


                                    Rectangle rr1 = new Rectangle();
                                    rr1.Width = _colWidths[idx];
                                    rr1.Height = _rowHeights[rowidx];
                                    rr1.Stroke = _gridCellOutline;
                                    rr1.StrokeThickness = 1;
                                    Canvas.SetLeft(rr1, left - _gridXShift);
                                    Canvas.SetTop(rr1, top);

                                    TheCanvas.Children.Add(rr1);

                                    string theText = (cellValue?.ToString() + "").ToUpper().Trim();
                                    bool isBooleanValue = (theText == "TRUE" || theText == "FALSE" ||
                                                          theText == "YES" || theText == "NO");

                                    if (isBooleanValue && _renderBooleansAsImages)
                                    {
                                        // Create an image control.
                                        var image = new Image();

                                        if (theText == "TRUE" || theText == "YES")
                                        {
                                            image.Source = _checkMark;
                                            image.Width = _rowHeights[rowidx] - 2;
                                            image.Height = _rowHeights[rowidx] - 2;
                                        }
                                        else
                                        {
                                            image.Source = _redX;
                                            image.Width = _rowHeights[rowidx] - 2;
                                            image.Height = _rowHeights[rowidx] - 2;
                                        }

                                        // Set the position of the image on the canvas.

                                        var leftoffset = (_colWidths[idx] - _rowHeights[rowidx]) / 2;
                                        Canvas.SetLeft(image, left + leftoffset - _gridXShift);
                                        Canvas.SetTop(image, top + 1);

                                        // Add the image to the canvas.
                                        TheCanvas.Children.Add(image);
                                    }
                                    else
                                    {

                                        TextBlock ttb = new TextBlock();
                                        ttb.FontSize = _gridFontSize;
                                        ttb.Foreground = tcb;
                                        ttb.FontFamily = _gridTypeface.FontFamily;
                                        ttb.FontWeight = _gridTypeface.Weight;
                                        ttb.FontStyle = _gridTypeface.Style;

                                        // Here we look for idx in JustifyColumns
                                        // if its there we might pad the strings placement
                                        // to the right to give it a right justification

                                        int leftoffset = 2;

                                        string thestringtoprint = cellValue?.ToString() + "";

                                        // If this is a boolean value and we're not rendering as images,
                                        // convert to 1/0 representation
                                        if (isBooleanValue && !_renderBooleansAsImages)
                                        {
                                            thestringtoprint = (theText == "TRUE" || theText == "YES") ? "1" : "0";
                                        }

                                        if (_truncateColumns.Contains(idx))
                                        {
                                            // here we need to truncate the string being rendered
                                            if (thestringtoprint.Length > _truncateColumnLength )
                                            {
                                                thestringtoprint = thestringtoprint.Substring(0, _truncateColumnLength) + "...";
                                            }

                                        }

                                        ttb.Text = thestringtoprint;

                                        if (JustifyColumns.Contains(idx))
                                        {
                                            var theTextFormatted =
                                            new FormattedText(thestringtoprint,
                                            CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                            _gridTypeface, _gridFontSize,
                                            _gridCellBrush);

                                            leftoffset = _colWidths[idx] - (int)theTextFormatted.Width - 2;
                                        }


                                        Canvas.SetLeft(ttb, left + leftoffset - _gridXShift);
                                        Canvas.SetTop(ttb, top + 1);
                                        TheCanvas.Children.Add(ttb);
                                        lastCellItemText = ttb.Text;
                                    }
                                }

                                left += _colWidths[idx];

                                idx++;
                            }

                            top += _rowHeights[rowidx];
                            rowidx++;
                        }

                    }
                }

                #endregion

                // Setup the Verticle Scrollbar
                // Here we will only scroll whole rows by the setting
                // the max value of the scrollbar to be the number of rows total in the grid

                // Only update scrollbar values if we're not handling a user scroll event
                if (!_isHandlingScrollEvent)
                {
                    _updatingScrollbars = true; // Prevent scroll event handlers from firing

                    TheVerticleScrollBar.Minimum = 0;
                    TheVerticleScrollBar.Maximum = _items.Count;
                    TheVerticleScrollBar.Value = _gridYShift;

                    // Setup the Horizontal Scrollbar
                    // The horizontal scrollbar uses a percentage-based system (0-100)
                    TheHorizontalScrollBar.Minimum = 0;
                    TheHorizontalScrollBar.Maximum = 100;

                    // Synchronize scrollbar value with _gridXShift
                    // Calculate what percentage the current shift represents
                    int totalWidth = 0;
                    if (_colWidths != null && _colWidths.Length > 0)
                    {
                        for (int i = 0; i < _colWidths.Length; i++)
                        {
                            totalWidth += _colWidths[i];
                        }

                        if (totalWidth > 0 && _gridXShift >= 0)
                        {
                            // Convert _gridXShift to percentage (0-100)
                            double percentage = (_gridXShift * 100.0) / totalWidth;
                            TheHorizontalScrollBar.Value = Math.Min(100, Math.Max(0, percentage));
                        }
                        else
                        {
                            TheHorizontalScrollBar.Value = 0;
                        }
                    }
                    else
                    {
                        TheHorizontalScrollBar.Value = 0;
                    }

                    _updatingScrollbars = false; // Re-enable scroll event handlers
                }

                //if (this.showCrossHairs)
                //{
                //    RenderCrossHairs();
                //}

                // Figure out the ROW we are on

                RecalcItemUnderMouse();

                GridHover?.Invoke(this, TheItemUnderTheMouse);

                // Force visual update to ensure the canvas redraws
                TheCanvas.InvalidateVisual();
            }


        }
        catch (Exception ex)
        {
            //Debug.WriteLine(ex.Message);
        }
    }

    #endregion
}
