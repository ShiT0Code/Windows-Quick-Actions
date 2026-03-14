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
    public SideBarCurrentData Data { get; set; }

    public RightBar(Windows.Graphics.RectInt32 rect, AppWindow paneAppWindow,SideBarCurrentData data)
    {
        Data = data;
        Data.RightPaneCurrentPoint = new(ScreenWidth, (int)(ScreenHeigh / 2 - (Data.HalfofHeight+10)));

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

    private async void Rectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
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

        if (Data.RightPaneCurrentPoint.X + deltaX > ScreenWidth - (Data.Width+50*ScalePercent) && Data.RightPaneCurrentPoint.X + deltaX < ScreenWidth)
            Data.RightPaneCurrentPoint = new((int)(Data.RightPaneCurrentPoint.X + deltaX), Data.RightPaneCurrentPoint.Y);
        if (Data.RightPaneCurrentPoint.Y + deltaY > 0 && Data.RightPaneCurrentPoint.Y + deltaY < ScreenHeigh - 56 * ScalePercent)
            Data.RightPaneCurrentPoint = new(Data.RightPaneCurrentPoint.X, (int)(Data.RightPaneCurrentPoint.Y + deltaY));
        RightTargetAppWindow.Move(Data.RightPaneCurrentPoint);
        await Task.Delay(150);
        SelectRightPane();
    }

    private async void Rectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        SelectRightPane();
        if (Data.RightPaneCurrentPoint.X <= ScreenWidth - Data.Width/2)
        {
            Data.RightPaneCurrentPoint = new((int)(ScreenWidth - (Data.Width + 10*ScalePercent)), Data.RightPaneCurrentPoint.Y);
            Data.IsRightPaneOpen = true;
        }
        else
        {
            Data.RightPaneCurrentPoint = new(ScreenWidth, Data.RightPaneCurrentPoint.Y);
            Data.IsRightPaneOpen = false;
        }
        RightTargetAppWindow.Move(Data.RightPaneCurrentPoint);
        rectangle.Opacity = Data.IsRightPaneOpen ? 0 : 1;
        await Task.Delay(160);
        SelectRightPane();
    }

    private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e) => Tapped();
    private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => Tapped();
    private async void Tapped()
    {
        SelectRightPane();
        Data.RightPaneCurrentPoint = new(Data.IsRightPaneOpen ? ScreenWidth : (int)(ScreenWidth - (Data.Width+10*ScalePercent)), Data.RightPaneCurrentPoint.Y);
        RightTargetAppWindow.Move(Data.RightPaneCurrentPoint);
        Data.IsRightPaneOpen = !Data.IsRightPaneOpen;
        rectangle.Opacity = Data.IsRightPaneOpen ? 0 : 1;
        await Task.Delay(160);
        SelectRightPane();
    }
}