using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using TouchHelper.Settings.SideBar;
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


    private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => aboutCard.Description = $"{Package.Current.DisplayName} ©2026 {Package.Current.PublisherDisplayName}\n{Package.Current.Id.Version}";

    private void Github_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => _ = Windows.System.Launcher.LaunchUriAsync(new System.Uri("https://github.com/ShiT0Code/Windows-Quick-Actions/tree/master"));
}
