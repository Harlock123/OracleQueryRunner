using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LAWgrid;

public partial class LAWgrid : UserControl
    {
        #region Private Variables

        private string _gridTitle = "The Grid Control for Avalonia";
        private int _gridTitleFontSize = 20;
        private int _gridTitleHeight = 10;
        private int _theLineLabelSize = 10;
        private int _theDataLabelSize = 10;
        private int _cellPaddingWidth = 10;
        private int _cellPaddingHeight = 10;
        private IBrush _gridBackground = Brushes.Cornsilk;
        private IBrush _gridCellOutline = Brushes.Black;
        private IBrush _gridCellContentBrush = Brushes.Black;
        private IBrush _gridCellBrush = Brushes.Wheat;

        private IBrush _gridCellHighlightBrush = Brushes.LightBlue;
        private IBrush _gridSelectedItemBrush = Brushes.AliceBlue;
        private IBrush _gridCellHighlightContentBrush = Brushes.Black;

        private bool _greenBarMode = false;
        private IBrush _greenBarColor1 = Brushes.White;
        private IBrush _greenBarColor2 = Brushes.PaleGreen;

        private IBrush _gridTitleBrush = Brushes.White;
        private IBrush _gridHeaderBrush = Brushes.DarkBlue;
        private IBrush _gridTitleBackground = Brushes.Blue;
        private IBrush _gridHeaderBackground = Brushes.Cyan;
        private Typeface _gridTitleTypeface = new Typeface("Arial", FontStyle.Normal, FontWeight.Bold);
        private Typeface _gridHeaderTypeface = new Typeface("Arial");
        private Typeface _gridTypeface = new Typeface("Arial");
        private int _gridheaderFontSize = 14;
        private int _gridFontSize = 12;

        private bool _showCrossHairs = true;
        private IBrush _crossHairBrush = Brushes.Red;

        private int _gridWidth = 800;
        private int _gridHeight = 300;

        private int _gridXShift = 0;
        private int _gridYShift = 0;

        private Point _lastPosition;

        private int _curMouseX;
        private int _curMouseY;
        private int _curMouseRow;
        private int _curMouseCol;
        private bool _mouseInControl;
        private int _scrollMultiplier = 3;
        private bool _suspendRendering;
        private bool _autosizeCellsToContents = true;
        private bool _updatingScrollbars = false;
        private bool _isHandlingScrollEvent = false;

        private bool _populateWithTestData;

        private int _gridHeaderAndTitleHeight;


        private List<object> _items = new List<object>();
        private List<object> _selecteditems = new List<object>();

        private int[] _colWidths;
        private int[] _rowHeights;

        private int _gridRows;
        private int _gridCols;

        private object _itemUnderMouse = null;

        private bool _inDesignMode;

        public GridHoverItem TheItemUnderTheMouse = new GridHoverItem();
        public GridHoverItem TheItemUnderTheMouseLast = new GridHoverItem();

        private DispatcherTimer _doubleClickTimer;
        private int _clickCounter;

        private Bitmap _checkMark;
        private Bitmap _redX;
        private Bitmap _folder;
        private Bitmap _file;

        private List<int> _justifyColumns = new List<int>();

        private List<int> _truncateColumns = new List<int>();
        private int _truncateColumnLength = 30;
        private bool _renderBooleansAsImages = true;
        private int _gridLeftOffset; // Platform-specific offset: 0 for macOS, 5 for others
        #endregion

        #region Constructor
        public LAWgrid()
        {
            InitializeComponent();

            _inDesignMode = Design.IsDesignMode;

            // // Set platform-specific left offset: 0 for macOS, 5 for Windows/Linux
            // _gridLeftOffset = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 0 : 5;

            _gridLeftOffset = 0;
            
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

            TheHorizontalScrollBar.Scroll += TheHorizontalScrollBar_scroll;
            TheVerticleScrollBar.Scroll += TheVerticleScrollBar_Scroll;

            TheHorizontalScrollBar.Value = 0.0;
            TheVerticleScrollBar.Value = 0.0;

            PointerWheelChanged += OnPointerWheelChanged;

            PointerMoved += OnPointerMoved;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;

            TheCanvas.SizeChanged += OnCanvasSizeChanged;

            _doubleClickTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _doubleClickTimer.Tick += DoubleClickTimer_Tick;

            _checkMark = LoadImage(ImageStrings.CheckMark);
            _redX = LoadImage(ImageStrings.RedX);
            _folder = LoadImage(ImageStrings.Folder);
            _file = LoadImage(ImageStrings.Afile);

            _items.Clear();


            if (_populateWithTestData || _inDesignMode)
                PopulateTestData();

            ReRender();
        }

        #endregion

        #region Events Exposed

        public event EventHandler<GridHoverItem> GridHover;

        public event EventHandler<GridHoverItem> GridItemDoubleClick;

        public event EventHandler<GridHoverItem> GridItemClick;

        #endregion

    }
