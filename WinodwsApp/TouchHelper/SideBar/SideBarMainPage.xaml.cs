using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace TouchHelper.SideBar;
public sealed partial class SideBarMainPage : Page
{
    public SideBarMainPage() => InitializeComponent();

    private void AllItems_Button_Click(object sender, RoutedEventArgs e)
        => this.Frame.Navigate(typeof(AllItemsPage), null, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
}