using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

public static class DisplayHelper
{
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
        scalePercent = Math.Round(dpiX / 96.0);
    }

    // 导入 dwmapi.dll 中的 DwmSetWindowAttribute 函数
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, ref int pvAttribute, uint cbAttribute);

    // DWMWA 属性常量：窗口圆角偏好 (Windows 11 22000+)
    private const uint DWMWA_WINDOW_CORNER_PREFERENCE = 33;

    public static void DisableWindowRounding(IntPtr hWnd)
    {
        try
        {
            // 设置不圆角
            int cornerPreference = 1;
            int hr = DwmSetWindowAttribute(hWnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref cornerPreference, sizeof(uint));
        }
        catch (Exception ex)
        {
            // 捕获可能的异常（如 DllNotFoundException）
            System.Diagnostics.Debug.WriteLine($"Error disabling window rounding: {ex.Message}");
        }
    }

}