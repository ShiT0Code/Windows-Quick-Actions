using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using WinRT.Interop;
using static TouchHelper.SideBar.SideBarDataContainer;

namespace TouchHelper.SideBar;

public sealed partial class RightBar : Window
{
    private nint hwnd;
    private LockedAxis _lockedAxis = LockedAxis.None;
    private bool _isLocked;
    private AppWindow RightTargetAppWindow;
    public static Windows.Graphics.PointInt32 PaneCurrentPosition_R = new();
    public static bool RightIsPaneOpen = false;

    public RightBar(Windows.Graphics.RectInt32 rect, AppWindow paneAppWindow)
    {
        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new WinUIEx.TransparentTintBackdrop();
        AppWindow.IsShownInSwitchers = false;
        RightTargetAppWindow = paneAppWindow;

        hwnd = WindowNative.GetWindowHandle(this);

        API_Helper.RemoveRoundedCorners(hwnd);
        API_Helper.RemoveWindowBorder(hwnd);
        API_Helper.RefreshWindowFrame(hwnd);
        API_Helper.SetWindowAlwaysOnTop(hwnd);
        AppWindow.MoveAndResize(rect);
        PaneCurrentPosition_R = new(ScreenWidth, (int)(ScreenHeigh / 2 - 300 * ScalePercent));

        InitializeComponent();
    }
    private void Window_Closed(object sender, WindowEventArgs args) => args.Handled = true;

    private void Rectangle_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
    {
        _isLocked = false;
        _lockedAxis = LockedAxis.None;
        SelectRightPane();
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
            double threshold = 5.0 * ScalePercent; // 灵敏度阈值（像素），避免微小抖动

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
                deltaY = 0 * ScalePercent;
            else if (_lockedAxis == LockedAxis.Y)
                deltaX = 0 * ScalePercent;
        }

        if (PaneCurrentPosition_R.X + deltaX > ScreenWidth - 468 * ScalePercent && PaneCurrentPosition_R.X + deltaX < ScreenWidth)
            PaneCurrentPosition_R.X += (int)deltaX;
        if (PaneCurrentPosition_R.Y + deltaY > 0 * ScalePercent && PaneCurrentPosition_R.Y + deltaY < ScreenHeigh - 56 * ScalePercent)
            PaneCurrentPosition_R.Y += (int)deltaY;
        RightTargetAppWindow.Move(PaneCurrentPosition_R);
        SelectRightPane();
    }

    private async void Rectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        SelectRightPane();
        if (PaneCurrentPosition_R.X <= ScreenWidth - 180 * ScalePercent)
        {
            PaneCurrentPosition_R.X = (int)(ScreenWidth - 374 * ScalePercent);
            RightIsPaneOpen = true;
        }
        else
        {
            PaneCurrentPosition_R.X = ScreenWidth;
            RightIsPaneOpen = false;
        }
        RightTargetAppWindow.Move(PaneCurrentPosition_R);
        rectangle.Opacity = RightIsPaneOpen ? 0 : 1;
        await Task.Delay(50);
        SelectRightPane();
        await Task.Delay(200);
        SelectRightPane();
    }

    private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e) => Tapped();
    private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => Tapped();
    private async void Tapped()
    {
        SelectRightPane();
        PaneCurrentPosition_R.X = RightIsPaneOpen ? ScreenWidth : (int)(ScreenWidth - 374 * ScalePercent);
        RightTargetAppWindow.Move(PaneCurrentPosition_R);
        RightIsPaneOpen = !RightIsPaneOpen;
        rectangle.Opacity = RightIsPaneOpen ? 0 : 1;
        await Task.Delay(50);
        SelectRightPane();
        await Task.Delay(200);
        SelectRightPane();
    }
}