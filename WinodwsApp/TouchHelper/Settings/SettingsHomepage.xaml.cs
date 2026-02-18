using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using TouchHelper.Settings.SideBar;
using TouchHelper.DataCore;

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

    private void TestData_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => DataContainer.Start();

    private async void Save_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await DataContainer.SaveFixedItems();
    }

    private async void Load_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await DataContainer.LoadFixedJsons();
    }
}
