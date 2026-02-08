using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
namespace MainApp.Settings;

public sealed partial class SettingsHomePage : Page
{
    public SettingsHomePage() => InitializeComponent();

    private void SideBar_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SettingsWindowsUI.Titles.Add(new() { Title = "侧边栏" });
        this.Frame.Navigate(typeof(SideBarSettingsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void SuspendedBall_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SettingsWindowsUI.Titles.Add(new() { Title = "悬浮球" });
        this.Frame.Navigate(typeof(SuspendedBallSettingsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void QuickSettings_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SettingsWindowsUI.Titles.Add(new() { Title = "快捷设置" });
        this.Frame.Navigate(typeof(QuickSettingsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }
}
