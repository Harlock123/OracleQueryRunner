using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace LAWgrid;

public partial class LAWgrid
{
    #region Private Helper Methods

    private void RecalcItemUnderMouse()
    {
        int offsety = 0;

        for (int i = _gridYShift; i < _gridRows; i++)
        {
            offsety += _rowHeights[i];

            if (_curMouseY - _gridHeaderAndTitleHeight < offsety)
            {
                TheItemUnderTheMouse.rowID = i;
                break;
            }
        }

        // Figure out the COLUMN we are on

        offsety = 0;
        for (int i = 0; i < _gridCols; i++)
        {
            offsety += _colWidths[i];


            if (_lastPosition.X + _gridXShift < offsety)
            {
                TheItemUnderTheMouse.colID = i;
                if (_items.Count > 0)
                    TheItemUnderTheMouse.ItemUnderMouse = _items[TheItemUnderTheMouse.rowID];
                break;
            }
        }

        // figure out the value of whats being hovered over
        TheItemUnderTheMouse.cellContent = "";

        if (Items.Count > 0)
        {
            object theitem = TheItemUnderTheMouse.ItemUnderMouse; //this.items[this.TheItemUnderTheMouse.rowID];
            int idx = 0;
            foreach (PropertyInfo property in Items[0].GetType().GetProperties())
            {
                if (idx == TheItemUnderTheMouse.colID)
                {
                    TheItemUnderTheMouse.cellContent =
                        property.GetValue(Items[TheItemUnderTheMouse.rowID])?.ToString() + "";
                    break;
                }

                idx++;
            }
        }
    }

    private List<PropertyInfoModel> GetObjectSchema(object obj)
    {
        List<PropertyInfoModel> schema = new List<PropertyInfoModel>();

        // Check if the object is an ExpandoObject (or IDictionary<string, object>)
        if (obj is IDictionary<string, object> dictionary)
        {
            // Handle dynamic objects (like ExpandoObject from SQL queries)
            foreach (var kvp in dictionary)
            {
                Type valueType = kvp.Value?.GetType() ?? typeof(string);
                schema.Add(new PropertyInfoModel { Name = kvp.Key, Type = valueType });
            }
        }
        else
        {
            // Handle regular objects with properties
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                schema.Add(new PropertyInfoModel { Name = property.Name, Type = property.PropertyType });
            }
        }

        return schema;
    }

    private void RenderCrossHairs()
    {
        if (_curMouseY >= TheCanvas.Bounds.Height || _curMouseX >= TheCanvas.Bounds.Width)
            return;

        Line l1 = new Line();
        l1.Stroke = _crossHairBrush;
        l1.StrokeThickness = 1;
        l1.StartPoint = new Point(0, _curMouseY);
        l1.EndPoint = new Point(TheCanvas.Bounds.Width, _curMouseY);

        TheCanvas.Children.Add(l1);

        Line l2 = new Line();
        l2.Stroke = _crossHairBrush;
        l2.StrokeThickness = 1;
        l2.StartPoint = new Point(_curMouseX, 0);
        l2.EndPoint = new Point(_curMouseX, TheCanvas.Bounds.Height);

        TheCanvas.Children.Add(l2);
    }

    private async Task<Bitmap> LoadImageAsync(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);

        using (var memoryStream = new MemoryStream(bytes))
        {
            return await Task.Run(() => Bitmap.DecodeToWidth(memoryStream, 32));
        }
    }

    private  Bitmap LoadImage(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);

        using (var memoryStream = new MemoryStream(bytes))
        {
            return Bitmap.DecodeToWidth(memoryStream, 32);
        }
    }

    #endregion
}
