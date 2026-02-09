using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace MainApp.Settings;
public sealed partial class SideBarItemManagePage : Page
{
    public SideBarItemManagePage() => InitializeComponent();
    public List<MainAction> Actions { get; set; } = [];
    public MainAction Action { get; set; } = new();

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (MainActionListViewItem)((ListView)sender).SelectedItem;
        if (item != null)
        {
            string str = item.Title;
            Action.Title = str;
        }
        else
            list.Items.RemoveAt(0);
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        list.Items.Add(new MainActionListViewItem { Title = "新操作项" });
    }
}
