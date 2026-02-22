using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WinRT.Interop;

namespace TouchHelper;
public partial class App : Application
{
    private nint _settingsWindowHwnd = IntPtr.Zero;

    private nint _sideBar_L_Hwnd = IntPtr.Zero;
    private nint _sideBar_R_Hwnd = IntPtr.Zero;

    public App() => InitializeComponent();

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var mainInstance = AppInstance.FindOrRegisterForKey("main");
        if (!mainInstance.IsCurrent)
        {
            var activationArgs = mainInstance.GetActivatedEventArgs();
            await mainInstance.RedirectActivationToAsync(activationArgs);

            App.Current.Exit();
            return;
        }
        AppInstance.GetCurrent().Activated += OnActivated;

        Window window = new()
        {
            ExtendsContentIntoTitleBar = true
        };
        window.Activate();
        _settingsWindowHwnd = WindowNative.GetWindowHandle(window);
        window.SystemBackdrop = new MicaBackdrop();
        window.Title = "设置";
        window.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        await Task.Delay(60);
        window.Content = new Settings.SettingsWindowUI();
    }

    private void OnActivated(object? sender, AppActivationArguments e)
    {
        if (IsIconic(_settingsWindowHwnd))
            ShowWindow(_settingsWindowHwnd, 9);
    }

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}