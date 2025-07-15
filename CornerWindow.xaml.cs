using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace ReRoundify
{
    public partial class CornerWindow : Window
    {
        // === Win32 API Declarations ===
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")] private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")] private static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        // === Constants ===
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;   // Hides from Alt-Tab.
        private const int WS_EX_TRANSPARENT = 0x00000020;  // Makes the window click-through.
        private const int WS_EX_LAYERED = 0x00080000;     // The prerequisite style for transparency.

        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        // === Final Hybrid Solution Components ===
        private static readonly List<IntPtr> _ourWindowHandles = new();
        private static readonly HashSet<string> _ignoreClassNames = new() { "MultitaskingViewFrame" }; // The "Ignore List" for Alt-Tab.
        private readonly DispatcherTimer _reinforcementTimer;
        private int _reinforcementTicksRemaining;
        private IntPtr _hookID;
        private readonly WinEventDelegate _winEventDelegate;

        public CornerWindow(double radius)
        {
            InitializeComponent();
            UpdateCornerRadius(radius);
            
            _winEventDelegate = new WinEventDelegate(WinEventProc);
            this.Closed += OnWindowClosed;

            _reinforcementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            _reinforcementTimer.Tick += ReinforcementTimer_Tick;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var hWnd = new WindowInteropHelper(this).Handle;
            var extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

            SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            
            _ourWindowHandles.Add(hWnd);
            
            ForceTopmost();
            _hookID = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (_ourWindowHandles.Contains(hwnd)) return;

            var sb = new StringBuilder(256);
            GetClassName(hwnd, sb, sb.Capacity);
            if (_ignoreClassNames.Contains(sb.ToString())) return;
            
            // If it's a window we need to fight, start the reinforcement process.
            this.Dispatcher.BeginInvoke(new Action(StartReinforcement));
        }

        private void ForceTopmost()
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            if (hWnd != IntPtr.Zero)
            {
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        private void StartReinforcement()
        {
            // Set a countdown. 150 ticks at 10ms = 1.5 seconds of reinforcement.
            _reinforcementTicksRemaining = 150; 
            ForceTopmost();
            _reinforcementTimer.Start();
        }

        private void ReinforcementTimer_Tick(object? sender, EventArgs e)
        {
            if (_reinforcementTicksRemaining > 0)
            {
                ForceTopmost();
                _reinforcementTicksRemaining--;
            }
            else
            {
                _reinforcementTimer.Stop();
            }
        }
        
        private void OnWindowClosed(object? sender, EventArgs e)
        {
            _reinforcementTimer.Stop();
            UnhookWinEvent(_hookID);
            _ourWindowHandles.Remove(new WindowInteropHelper(this).Handle);
        }

        public void UpdateCornerRadius(double radius)
        {
            if (radius <= 0) { this.Visibility = Visibility.Collapsed; return; }

            this.Visibility = Visibility.Visible;
            var size = new Size(radius, radius);
            
            var tl_figure = new PathFigure { StartPoint = new Point(0, 0) };
            tl_figure.Segments.Add(new LineSegment { Point = new Point(radius, 0) });
            tl_figure.Segments.Add(new ArcSegment { Point = new Point(0, radius), Size = size, SweepDirection = SweepDirection.Counterclockwise, IsLargeArc = false });
            tl_figure.IsClosed = true;
            TopLeftCorner.Data = new PathGeometry(new[] { tl_figure });

            var tr_figure = new PathFigure { StartPoint = new Point(0, 0) };
            tr_figure.Segments.Add(new LineSegment { Point = new Point(radius, 0) });
            tr_figure.Segments.Add(new LineSegment { Point = new Point(radius, radius) });
            tr_figure.Segments.Add(new ArcSegment { Point = new Point(0, 0), Size = size, SweepDirection = SweepDirection.Counterclockwise, IsLargeArc = false });
            TopRightCorner.Data = new PathGeometry(new[] { tr_figure });
            
            var bl_figure = new PathFigure { StartPoint = new Point(0, 0) };
            bl_figure.Segments.Add(new LineSegment { Point = new Point(0, radius) });
            bl_figure.Segments.Add(new LineSegment { Point = new Point(radius, radius) });
            bl_figure.Segments.Add(new ArcSegment { Point = new Point(0, 0), Size = size, SweepDirection = SweepDirection.Clockwise, IsLargeArc = false });
            BottomLeftCorner.Data = new PathGeometry(new[] { bl_figure });

            var br_figure = new PathFigure { StartPoint = new Point(radius, 0) };
            br_figure.Segments.Add(new LineSegment { Point = new Point(radius, radius) });
            br_figure.Segments.Add(new LineSegment { Point = new Point(0, radius) });
            br_figure.Segments.Add(new ArcSegment { Point = new Point(radius, 0), Size = size, SweepDirection = SweepDirection.Counterclockwise, IsLargeArc = false });
            BottomRightCorner.Data = new PathGeometry(new[] { br_figure });
        }
    }
}