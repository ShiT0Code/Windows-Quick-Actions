using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace TouchHelper.SideBar;

static class SideBarDataContainer
{
    public static int ScreenWidth { get; set; } = 0;
    public static int ScreenHeigh { get; set; } = 0;
    public static double ScalePercent { get; set; } = 0;
    public enum LockedAxis { X, Y, None };
    public static RectInt32 LeftPaneCloseRect { get; set; }
    public static PointInt32 LeftPaneOpenosition { get; set; }
    public static RectInt32 RightPaneCloseRect { get; set; }
    public static PointInt32 RightPaneOpenPosition { get; set; }

    public static nint LeftPaneHwnd { get; set; }
    public static nint RightPaneHwnd { get; set; }


    public static void LaunchSideBar()
    {
        var tempWindow = new Window();
        var hwnd = WindowNative.GetWindowHandle(tempWindow);
        API_Helper.GetMonitorInfoFromWindow(hwnd, out int screenWidth, out int screenHeigh, out double scalePercent);
        ScreenHeigh = screenHeigh;
        ScreenWidth = screenWidth;
        ScalePercent = scalePercent;

        LeftPaneCloseRect = new((int)(-378 * ScalePercent), (int)(ScreenHeigh / 2 - 300 * ScalePercent), (int)(368 * scalePercent), (int)(600 * scalePercent));
        LeftPaneOpenosition = new(0, (int)(ScreenHeigh / 2 - 300 * ScalePercent));
        RightPaneCloseRect = new(ScreenWidth, (int)(ScreenHeigh / 2 - 300 * ScalePercent), (int)(368 * scalePercent), (int)(600 * scalePercent));
        RightPaneOpenPosition = new((int)(ScreenWidth - 368 * ScalePercent), (int)(ScreenHeigh / 2 - 300 * ScalePercent));

        RectInt32 leftBarRect = new((int)(-110 * ScalePercent), (int)(ScreenHeigh / 2 - 40 * ScalePercent), (int)(120 * ScalePercent), (int)(80 * ScalePercent));
        RectInt32 rightBarRect = new((int)(ScreenWidth - 12 * ScalePercent), (int)(ScreenHeigh / 2 - 40 * ScalePercent), 120, (int)(80 * ScalePercent));

        var leftPane = new SideBarTargetWindow(SideBarTargetWindow.PaneWindowType.Left, LeftPaneCloseRect);
        LeftPaneHwnd = WindowNative.GetWindowHandle(leftPane);
        leftPane.Activate();
        new LeftBar(leftBarRect, leftPane.AppWindow).Activate();
        var rightPane = new SideBarTargetWindow(SideBarTargetWindow.PaneWindowType.Right, RightPaneCloseRect);
        RightPaneHwnd = WindowNative.GetWindowHandle(rightPane);
        rightPane.Activate();
        new RightBar(rightBarRect, rightPane.AppWindow).Activate();
    }

    public static void SelectLeftPane()
    {
        API_Helper.BringToTop(LeftPaneHwnd);
    }

    public static void SelectRightPane()
    {
        API_Helper.BringToTop(RightPaneHwnd);
    }

}