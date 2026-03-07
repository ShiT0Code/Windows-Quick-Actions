using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using WinRT.Interop;
using static TouchHelper.SideBar.SideBarDataContainer;

namespace TouchHelper.SideBar;

public sealed partial class LeftBar : Window
{
    private nint hwnd;
    private LockedAxis _lockedAxis = LockedAxis.None;
    private bool _isLocked;
    private AppWindow LeftTargetAppWindow;
    public SideBarCurrentData CurDa { get; set; }

    public LeftBar(Windows.Graphics.RectInt32 rect, AppWindow paneAppWindow, SideBarCurrentData data)
    {
        CurDa = data;
        CurDa.LeftPaneCurrentPoint = new((int)(-276 * ScalePercent), (int)(ScreenHeigh / 2 - 225 * ScalePercent));

        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new WinUIEx.TransparentTintBackdrop();
        AppWindow.IsShownInSwitchers = false;
        LeftTargetAppWindow = paneAppWindow;

        hwnd = WindowNative.GetWindowHandle(this);

        API_Helper.RemoveRoundedCorners(hwnd);
        API_Helper.RemoveWindowBorder(hwnd);
        API_Helper.RefreshWindowFrame(hwnd);
        API_Helper.SetWindowAlwaysOnTop(hwnd);
        AppWindow.MoveAndResize(rect);

        InitializeComponent();
    }
    private void Window_Closed(object sender, WindowEventArgs args) => args.Handled = true;

    private void Rectangle_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
    {
        _isLocked = false;
        _lockedAxis = LockedAxis.None;
        SelectLeftPane();
        rectangle.Opacity = 0;
    }

    private void Rectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        double deltaX = e.Delta.Translation.X;
        double deltaY = e.Delta.Translation.Y;

        // 如果尚未锁定轴向，根据累积位移判断锁定方向
        if (!_isLocked)
        {
            var cumX = e.Cumulative.Translation.X;
            var cumY = e.Cumulative.Translation.Y;
            double threshold = 5.0; // 灵敏度阈值（像素），避免微小抖动

            if (Math.Abs(cumX) > threshold || Math.Abs(cumY) > threshold)
            {
                // 比较累积位移绝对值，决定锁定方向
                if (Math.Abs(cumX) > Math.Abs(cumY))
                {
                    _lockedAxis = LockedAxis.X;
                }
                else
                    _lockedAxis = LockedAxis.Y;
                _isLocked = true;
            }
        }

        // 如果已锁定，将非锁定方向的增量清零
        if (_isLocked)
        {
            if (_lockedAxis == LockedAxis.X)
                deltaY = 0;
            else if (_lockedAxis == LockedAxis.Y)
                deltaX = 0;
        }

        if (CurDa.LeftPaneCurrentPoint.X + deltaX > -276 * ScalePercent && CurDa.LeftPaneCurrentPoint.X + deltaX < 48 * ScalePercent)
            CurDa.LeftPaneCurrentPoint = new((int)(CurDa.LeftPaneCurrentPoint.X + deltaX), CurDa.LeftPaneCurrentPoint.Y);
        if (CurDa.LeftPaneCurrentPoint.Y + deltaY > 0 && CurDa.LeftPaneCurrentPoint.Y + deltaY < ScreenHeigh - 56 * ScalePercent)
            CurDa.LeftPaneCurrentPoint = new(CurDa.LeftPaneCurrentPoint.X, (int)(CurDa.LeftPaneCurrentPoint.Y + deltaY));
        LeftTargetAppWindow.Move(CurDa.LeftPaneCurrentPoint);
        SelectLeftPane();
    }

    private async void Rectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        SelectLeftPane();
        if (CurDa.LeftPaneCurrentPoint.X >= -180 * ScalePercent)
        {
            CurDa.LeftPaneCurrentPoint = new(7, CurDa.LeftPaneCurrentPoint.Y);
            CurDa.IsLeftPaneOpen = true;
        }
        else
        {
            CurDa.LeftPaneCurrentPoint = new((int)(-276 * ScalePercent), CurDa.LeftPaneCurrentPoint.Y);
            CurDa.IsLeftPaneOpen = false;
        }
        LeftTargetAppWindow.Move(CurDa.LeftPaneCurrentPoint);
        rectangle.Opacity = CurDa.IsLeftPaneOpen ? 0 : 1;
        await Task.Delay(500);
        SelectLeftPane();
    }

    private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e) => Tapped();
    private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => Tapped();
    private async void Tapped()
    {
        SelectLeftPane();
        CurDa.LeftPaneCurrentPoint = new((CurDa.IsLeftPaneOpen ? (int)(-276 * ScalePercent) : 7), CurDa.LeftPaneCurrentPoint.Y);
        LeftTargetAppWindow.Move(CurDa.LeftPaneCurrentPoint);
        CurDa.IsLeftPaneOpen = !CurDa.IsLeftPaneOpen;
        rectangle.Opacity = CurDa.IsLeftPaneOpen ? 0 : 1;
        await Task.Delay(200);
        SelectLeftPane();
    }
}