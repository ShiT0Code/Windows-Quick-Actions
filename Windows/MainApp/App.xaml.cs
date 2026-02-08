using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace MainApp;
public partial class App : Application
{
    private Window? _window;

    public App() => InitializeComponent();

    protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var mainInstance = AppInstance.FindOrRegisterForKey("main");
        // 判断当前激活的事件是否已被处理
        // 如果这不是"main"实例，那么激活事件需要重定向
        if (!mainInstance.IsCurrent)
        {
            // 非主实例，进行重定向
            // 将当前获取到的激活事件参数重定向到已注册的"main"实例
            var activationArgs = mainInstance.GetActivatedEventArgs();
            await mainInstance.RedirectActivationToAsync(activationArgs);

            App.Current.Exit();
            return;
        }
        // 当前是主实例，正常启动应用
        AppInstance.GetCurrent().Activated += OnActivated;
        _window = new();
        _window.Activate();
        _window.ExtendsContentIntoTitleBar = true;
        _window.Content = new Settings.SettingsWindowsUI();
        _window.SystemBackdrop = new MicaBackdrop();
        _window.Title = "设置";
    }

    private void OnActivated(object? sender, AppActivationArguments e)
    {
        // 将窗口提到前台
        var dispatcherQueue = _window?.DispatcherQueue;
        dispatcherQueue?.TryEnqueue(() =>
        {
            if (_window != null)
            {
                var hwnd = WindowNative.GetWindowHandle(_window);
                SetForegroundWindow(hwnd);
                // 检查窗口句柄是否有效
                if (hwnd == IntPtr.Zero)
                {
                    _window.Activate();
                    return;
                }

                // 如果窗口被最小化，则将其还原
                if (IsIconic(hwnd))
                    ShowWindow(hwnd, 9);
            }
        });
    }

    /// 引用的外部方法
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    // 检查是否最小化并还原窗口
    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
