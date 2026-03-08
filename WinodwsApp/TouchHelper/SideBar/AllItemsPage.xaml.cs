using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TouchHelper.SideBar;
public sealed partial class AllItemsPage : Page
{
    public AllItemsPage() => InitializeComponent();

    private void Back_Button_Click(object sender, RoutedEventArgs e) => this.Frame.GoBack();
}
