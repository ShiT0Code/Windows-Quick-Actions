using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace MainApp.Settings;
public sealed partial class SideBarSettingsPage : Page
{
    public SideBarSettingsPage() => InitializeComponent();

    private void SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SettingsWindowsUI.Titles.Add("侧边栏固定项");
        this.Frame.Navigate(typeof(SideBarItemManagePage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }                     
}