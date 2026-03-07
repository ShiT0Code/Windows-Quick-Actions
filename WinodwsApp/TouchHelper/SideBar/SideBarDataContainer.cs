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

    // 定义侧边栏窗口大小
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

        SideBarCurrentData CurrentData = new();

        // 设定大小
        LeftPaneCloseRect = new((int)(-276 * ScalePercent), (int)(ScreenHeigh / 2 - 225 * ScalePercent), (int)(276 * scalePercent), (int)(450 * scalePercent));
        LeftPaneOpenosition = new(0, (int)(ScreenHeigh / 2 - 225 * ScalePercent));
        RightPaneCloseRect = new(ScreenWidth, (int)(ScreenHeigh / 2 - 225 * ScalePercent), (int)(276 * scalePercent), (int)(450 * scalePercent));
        RightPaneOpenPosition = new((int)(ScreenWidth - 276 * ScalePercent), (int)(ScreenHeigh / 2 - 225 * ScalePercent));

        RectInt32 leftBarRect = new((int)(-110 * ScalePercent), (int)(ScreenHeigh / 2 - 28 * ScalePercent), (int)(120 * ScalePercent), (int)(54 * ScalePercent));
        RectInt32 rightBarRect = new((int)(ScreenWidth - 10 * ScalePercent), (int)(ScreenHeigh / 2 - 28 * ScalePercent), 120, (int)(54 * ScalePercent));

        var leftPane = new SideBarWindow(SideBarWindow.PaneWindowType.Left, LeftPaneCloseRect, CurrentData);
        LeftPaneHwnd = WindowNative.GetWindowHandle(leftPane);
        leftPane.Activate();
        new LeftBar(leftBarRect, leftPane.AppWindow, CurrentData).Activate();
        var rightPane = new SideBarWindow(SideBarWindow.PaneWindowType.Right, RightPaneCloseRect, CurrentData);
        RightPaneHwnd = WindowNative.GetWindowHandle(rightPane);
        rightPane.Activate();
        new RightBar(rightBarRect, rightPane.AppWindow,CurrentData).Activate();
    }

    public static void SelectLeftPane() => API_Helper.BringToTop(LeftPaneHwnd);

    public static void SelectRightPane() => API_Helper.BringToTop(RightPaneHwnd);
}