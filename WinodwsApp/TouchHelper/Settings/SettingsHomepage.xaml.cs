using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using TouchHelper.Settings.SideBar;
using TouchHelper.SideBar;
using Windows.ApplicationModel;

namespace TouchHelper.Settings;
public sealed partial class SettingsHomePage : Page
{
    public SettingsHomePage() => InitializeComponent();

    private void ExclusionApps_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SettingsWindowUI.Titles.Add("排除的应用");
        this.Frame.Navigate(typeof(ExclusionAppsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void FixedItem_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.IsEnabled = false;
        SettingsWindowUI.Titles.Add("固定项");
        this.Frame.Navigate(typeof(FixedItemListPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }


    private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["enableSideBar"] is bool enableSideBar)
            sideBarToggleSwitch.IsOn = enableSideBar;
        else
            sideBarToggleSwitch.IsOn = false;
        sideBarToggleSwitch.Toggled += ToggleSwitch_Toggled;

        aboutCard.Description = $"{Package.Current.DisplayName} ©2026 {Package.Current.PublisherDisplayName}\n";
        var ver = Package.Current.Id.Version;
        aboutCard.Description += $"v{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
    }

    private void Github_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) 
        => _ = Windows.System.Launcher.LaunchUriAsync(new System.Uri("https://github.com/ShiT0Code/Windows-Quick-Actions/tree/master"));

    private void ToggleSwitch_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        bool enable = sideBarToggleSwitch.IsOn;
        Windows.Storage.ApplicationData.Current.LocalSettings.Values["enableSideBar"] = enable;
        if(enable)
        {
            sideBarToggleSwitch.IsOn = false;
            new SideBarWindow_Left().Activate();
            new SideBarWindow_Right().Activate();
        }
    }
}
