using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TouchHelper.SideBar;
using WinRT.Interop;

namespace TouchHelper;
public partial class App : Application
{
    private nint _settingsWindowHwnd;
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
        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["enableSideBar"] is bool enableSideBar && enableSideBar)
            SideBarDataContainer.LaunchSideBar();
    }

    private void OnActivated(object? sender, AppActivationArguments e)
    {
        if (API_Helper.IsIconic(_settingsWindowHwnd))
            API_Helper.BringToTop(_settingsWindowHwnd);
    }
}