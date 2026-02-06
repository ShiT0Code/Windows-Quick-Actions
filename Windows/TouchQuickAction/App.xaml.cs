using ABI.Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using WinRT.Interop;

namespace TouchQuickAction;

public partial class App : Application
{
    private Window? _settings_window;
    private nint _settings_hwnd;

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

            Current.Exit();
            return;
        }
        // 当前是主实例，正常启动应用
        AppInstance.GetCurrent().Activated += OnActivated;

        _settings_window = new SettingsWindow();
        _settings_window.Activate();
        _settings_hwnd = WindowNative.GetWindowHandle(_settings_window);
    }

    private void OnActivated(object? sender, AppActivationArguments e)
    {
        if (_settings_window != null)
        {
            // 将窗口提到前台
            var dispatcherQueue = _settings_window?.DispatcherQueue;
            dispatcherQueue?.TryEnqueue(() =>
            {
                SetForegroundWindow(_settings_hwnd);
                // 如果窗口被最小化，则将其还原
                if (IsIconic(_settings_hwnd))
                    ShowWindow(_settings_hwnd, 9);
            });
        }
        else
        {
            _settings_window = new SettingsWindow();
            _settings_window.Activate();
            _settings_hwnd = WindowNative.GetWindowHandle(_settings_window);
        }
    }

    // 引用的外部方法
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    // 检查是否最小化并还原窗口
    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}