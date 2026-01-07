using System.ComponentModel;
using Avalonia.Media;
using Avalonia.Controls;

namespace LAWgrid;

public partial class LAWgrid : UserControl
{
    #region Properties

    // this is the title of the grid
    [DefaultValue("The Grid Control for Avalonia")]
    public string GridTitle
    {
        get { return _gridTitle; }
        set
        {
            _gridTitle = value;
            ReRender();
        }
    }

    // this is the font size for the grid title
    [DefaultValue(20)]
    public int GridTitleFontSize
    {
        get { return _gridTitleFontSize; }
        set
        {
            _gridTitleFontSize = value;
            ReRender();
        }
    }

    // this is the font size for the grid headers (column names)
    [DefaultValue(14)]
    public int GridHeaderFontSize
    {
        get { return _gridheaderFontSize; }
        set
        {
            _gridheaderFontSize = value;
            ReRender();
        }
    }

    // this is the font size for the grid contents
    [DefaultValue(12)]
    public int GridFontSize
    {
        get { return _gridFontSize; }
        set
        {
            _gridFontSize = value;
            ReRender();
        }
    }

    // this is the height of the grid title in pixels
    [DefaultValue(10)]
    public int GridTitleHeight
    {
        get { return _gridTitleHeight; }
        set
        {
            _gridTitleHeight = value;
            ReRender();
        }
    }

    [DefaultValue(10)]
    public int CellPaddingWidth
    {
        get { return _cellPaddingWidth; }
        set
        {
            _cellPaddingWidth = value;
            ReRender();
        }
    }
    [DefaultValue(10)]
    public int CellPaddingHeight
    {
        get { return _cellPaddingHeight; }
        set
        {
            _cellPaddingHeight = value;
            ReRender();
        }
    }

    // this is the brush used to render the Grid Background
    [DefaultValue("Cornsilk")]
    public IBrush GridBackground
    {
        get { return _gridBackground; }
        set
        {
            _gridBackground = value;
            ReRender();
        }
    }

    // this is the brush that will be used to render the grid title font
    [DefaultValue("White")]
    public IBrush GridTitleBrush
    {
        get { return _gridTitleBrush; }
        set
        {
            _gridTitleBrush = value;
            ReRender();
        }
    }

    // this is the brush that will be used to render the grids title background
    [DefaultValue("Blue")]
    public IBrush GridTitleBackground
    {
        get { return _gridTitleBackground; }
        set
        {
            _gridTitleBackground = value;
            ReRender();
        }
    }

    // this is the brush that will be used to render the grids headers (column names)
    [DefaultValue("DarkBlue")]
    public IBrush GridHeaderBrush
    {
        get { return _gridHeaderBrush; }
        set
        {
            _gridHeaderBrush = value;
            ReRender();
        }
    }

    // this is the brush that will be used to fill the grid header
    [DefaultValue("Cyan")]
    public IBrush GridHeaderBackground
    {
        get { return _gridHeaderBackground; }
        set
        {
            _gridHeaderBackground = value;
            ReRender();
        }
    }

    // this is the brush that will be used to outline the grid cells
    [DefaultValue("Black")]
    public IBrush GridCellOutline
    {
        get { return _gridCellOutline; }
        set
        {
            _gridCellOutline = value;
            ReRender();
        }
    }

    // this is the brush that will be used to fill the grid cells
    [DefaultValue("Wheat")]
    public IBrush GridCellBrush
    {
        get { return _gridCellBrush; }
        set
        {
            _gridCellBrush = value;
            ReRender();
        }
    }

    // Hovering over a cell will highlight its background with this brush
    [DefaultValue("LightBlue")]
    public IBrush GridCellHighlightBrush
    {
        get { return _gridCellHighlightBrush; }
        set
        {
            _gridCellHighlightBrush = value;
            ReRender();
        }
    }

    // Hovering over a cell will highlight its content with this brush
    [DefaultValue("Black")]
    public IBrush GridCellHighlightContentBrush
    {
        get { return _gridCellHighlightContentBrush; }
        set
        {
            _gridCellHighlightContentBrush = value;
            ReRender();
        }
    }

    // SelectedItems in the grid will be highlighted with this brush
    [DefaultValue("AliceBlue")]
    public IBrush GridSelectedItemBrush
    {
        get { return _gridSelectedItemBrush; }
        set
        {
            _gridSelectedItemBrush = value;
            ReRender();
        }
    }

    // Enables alternating row colors (green bar mode)
    [DefaultValue(false)]
    public bool GreenBarMode
    {
        get { return _greenBarMode; }
        set
        {
            _greenBarMode = value;
            ReRender();
        }
    }

    // First alternating row color for GreenBarMode (even rows)
    [DefaultValue("White")]
    public IBrush GreenBarColor1
    {
        get { return _greenBarColor1; }
        set
        {
            _greenBarColor1 = value;
            ReRender();
        }
    }

    // Second alternating row color for GreenBarMode (odd rows)
    [DefaultValue("PaleGreen")]
    public IBrush GreenBarColor2
    {
        get { return _greenBarColor2; }
        set
        {
            _greenBarColor2 = value;
            ReRender();
        }
    }

    // this is the font definition for the grid title
    [DefaultValue("Arial, 12, Normal, Normal")]
    public Typeface GridTitleTypeface
    {
        get { return _gridTitleTypeface; }
        set
        {
            _gridTitleTypeface = value;
            ReRender();
        }
    }

    // this is the font definition for the grid contents
    [DefaultValue("Arial, 12, Normal, Normal")]
    public Typeface GridTypeface
    {
        get { return _gridTypeface; }
        set
        {
            _gridTypeface = value;
            ReRender();
        }
    }

    // this is the font definition for the grid header
    [DefaultValue("Arial, 14, Normal, Normal")]
    public Typeface GridHeaderTypeface
    {
        get { return _gridHeaderTypeface; }
        set
        {
            _gridHeaderTypeface = value;
            ReRender();
        }
    }

    // This is the data that will be displayed in the grid
    [DefaultValue(null)]
    public List<object> Items
    {
        get { return _items; }
        set
        {
            SuspendRendering = true;

            _items = value;
            _selecteditems.Clear();
            TheItemUnderTheMouse = new GridHoverItem();
            _gridXShift = 0;
            _gridYShift = 0;

            SuspendRendering = false;

            ReRender();
        }
    }

    // This is the data that will be displayed in the grid as selected rows
    [DefaultValue(null)]
    public List<object> SelectedItems
    {
        get { return _selecteditems; }
        set
        {
            _selecteditems = value;
            ReRender();
        }
    }

    // Flag to enable or disable rendering the grid
    [DefaultValue(false)]
    public bool SuspendRendering
    {
        get { return _suspendRendering; }
        set
        {
            _suspendRendering = value;
            ReRender();
        }
    }

    // Flag to enable or disable autosizing the cells to the contents
    [DefaultValue(true)]
    public bool AutosizeCellsToContents
    {
        get { return _autosizeCellsToContents; }
        set
        {
            _autosizeCellsToContents = value;
            ReRender();
        }
    }

    // The width of the grid in Pixels
    [DefaultValue(800)]
    public int GridWidth
    {
        get { return _gridWidth; }
        set
        {
            _gridWidth = value;
            ReRender();
        }
    }

    // The height of the grid in Pixels
    [DefaultValue(300)]
    public int GridHeight
    {
        get { return _gridHeight; }
        set
        {
            _gridHeight = value;
            ReRender();
        }
    }

    // Boolean to Show or Hide Crosshairs on hovering over a cell
    [DefaultValue(true)]
    public bool ShowCrossHairs
    {
        get { return _showCrossHairs; }
        set
        {
            _showCrossHairs = value;
            ReRender();
        }
    }

    // Boolean to Show or Hide A set of self contained test objects in the grid
    [DefaultValue(false)]
    public bool PopulateWithTestData
    {
        get { return _populateWithTestData; }
        set
        {
            _populateWithTestData = value;

            if (_populateWithTestData)
            {
                PopulateTestData();

            }
            else
            {
                Items.Clear();
            }

            ReRender();
        }
    }

    // An accelerator for Mouse Wheel scrolling Operations
    [DefaultValue(3)]
    public int ScrollMultiplier
    {
        get { return _scrollMultiplier; }
        set
        {
            _scrollMultiplier = value;
            ReRender();
        }
    }

    // The current row under the mouse
    [DefaultValue(0)]
    public int CurMouseRow
    {
        get { return _curMouseRow; }
        set
        {
            _curMouseRow = value;
            //this.ReRender();
        }
    }

    // The current column under the mouse
    [DefaultValue(0)]
    public int CurMouseCol
    {
        get { return _curMouseCol; }
        set
        {
            _curMouseCol = value;
            //this.ReRender();
        }
    }

    // list of columns that will need to be right justified
    [DefaultValue(null)]
    public List<int> JustifyColumns
    {
        get { return _justifyColumns; }
        set
        {
            _justifyColumns = value;
            ReRender();
        }
    }

    // list of columns that will need to be truncated at truncatecolumnlength
    [DefaultValue(null)]
    public List<int> TruncateColumns
    {
        get { return _truncateColumns; }
        set
        {
            _truncateColumns = value;
            ReRender();
        }
    }

    // The number of characters that columns that are being truncated
    // will be truncated at
    [DefaultValue(30)]
    public int TruncateColumnLength
    {
        get { return _truncateColumnLength; }
        set
        {
            _truncateColumnLength = value;
            ReRender();
        }
    }

    // Controls whether boolean values (TRUE/FALSE/YES/NO) are rendered as images
    // or as simple text (1/0/' ')
    [DefaultValue(true)]
    public bool RenderBooleansAsImages
    {
        get { return _renderBooleansAsImages; }
        set
        {
            _renderBooleansAsImages = value;
            ReRender();
        }
    }

    #endregion
}
