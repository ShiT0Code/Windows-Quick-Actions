using System;
using System.Runtime.InteropServices;

namespace TouchHelper;
internal static class API_Helper
{
    // 还原窗口
    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);

    // 获取显示设置
    // 常量定义
    private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
    private const int MDT_EFFECTIVE_DPI = 0;

    // 结构体定义
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFOEX
    {
        public int cbSize;
        public RECT rcMonitor;          // 显示器矩形区域（物理像素）
        public RECT rcWork;              // 工作区矩形区域
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;          // 设备名称
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    // Win32 API 导入
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [DllImport("shcore.dll")]
    private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    /// <summary>
    /// 获取窗口所在显示器的物理分辨率和缩放百分比
    /// </summary>
    /// <param name="hwnd">窗口句柄</param>
    /// <param name="width">输出：显示器物理宽度（像素）</param>
    /// <param name="height">输出：显示器物理高度（像素）</param>
    /// <param name="scalePercent">输出：缩放百分比（例如 150 表示 150%）</param>
    public static void GetMonitorInfoFromWindow(IntPtr hwnd, out int width, out int height, out double scalePercent)
    {
        width = 0;
        height = 0;
        scalePercent = 100;

        if (hwnd == IntPtr.Zero)
            throw new ArgumentException("窗口句柄不能为空", nameof(hwnd));

        // 1. 获取窗口所在显示器句柄
        IntPtr hMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        if (hMonitor == IntPtr.Zero)
            throw new InvalidOperationException("无法获取显示器句柄");

        // 2. 获取显示器信息（物理分辨率）
        MONITORINFOEX monitorInfo = new MONITORINFOEX();
        monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
        if (!GetMonitorInfo(hMonitor, ref monitorInfo))
            throw new InvalidOperationException("调用 GetMonitorInfo 失败");

        // 物理分辨率 = rcMonitor 的宽高
        width = monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left;
        height = monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top;

        // 3. 获取显示器 DPI
        uint dpiX, dpiY;
        int hr = GetDpiForMonitor(hMonitor, MDT_EFFECTIVE_DPI, out dpiX, out dpiY);
        if (hr != 0) // S_OK
            throw new InvalidOperationException("调用 GetDpiForMonitor 失败，HRESULT=" + hr);

        // 缩放百分比 = (dpiX / 96) * 100
        scalePercent = dpiX / 96.0;
    }

    // 去除窗口样式
    // Windows API 常量
    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;

    // 窗口样式
    private const uint WS_BORDER = 0x00800000;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_THICKFRAME = 0x00040000;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WS_MAXIMIZEBOX = 0x00010000;
    private const uint WS_MINIMIZEBOX = 0x00020000;

    // 扩展窗口样式
    private const uint WS_EX_CLIENTEDGE = 0x00000200;
    private const uint WS_EX_WINDOWEDGE = 0x00000100;
    private const uint WS_EX_DLGMODALFRAME = 0x00000001;
    private const uint WS_EX_STATICEDGE = 0x00020000;

    // DWM 属性
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWA_BORDER_COLOR = 34;
    private const int DWMWA_CAPTION_COLOR = 35;
    private const int DWMWA_USE_HOST_BACKDROP_BRUSH = 17; // 禁用系统背景效果

    // 圆角偏好
    private enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    // P/Invoke 声明
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr,
        ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr,
        ref DWM_WINDOW_CORNER_PREFERENCE attrValue, int attrSize);

    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private static readonly IntPtr HWND_TOP = IntPtr.Zero;
    public static void RemoveRoundedCorners(IntPtr hwnd)
    {
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
        DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE,
            ref preference, Marshal.SizeOf(typeof(int)));
    }

    public static void RemoveWindowBorder(IntPtr hwnd)
    {
        // 获取当前样式
        uint style = GetWindowLong(hwnd, GWL_STYLE);
        uint exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

        // 移除边框相关样式
        style &= ~(WS_BORDER | WS_CAPTION | WS_THICKFRAME |
                   WS_SYSMENU | WS_MAXIMIZEBOX | WS_MINIMIZEBOX);

        // 移除扩展边框样式
        exStyle &= ~(WS_EX_CLIENTEDGE | WS_EX_WINDOWEDGE |
                     WS_EX_DLGMODALFRAME | WS_EX_STATICEDGE);

        // 应用新样式
        SetWindowLong(hwnd, GWL_STYLE, style);
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
    }

    public static void RefreshWindowFrame(IntPtr hwnd)
    {
        // 强制窗口重绘边框
        SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0,
            SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE |
            SWP_NOZORDER | SWP_NOACTIVATE);
    }

    // SetWindowPos 相关常量
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    public const uint SWP_SHOWWINDOW = 0x0040;
    public static void SetWindowAlwaysOnTop(IntPtr hwnd) 
        => SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool BringWindowToTop(IntPtr hWnd);
    public static void BringToTop(IntPtr hwnd)
    {
        SetForegroundWindow(hwnd);
        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
        SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }
}