using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MainApp.Settings;
public sealed partial class SideBarItemManagePage : Page
{
    public SideBarItemManagePage() => InitializeComponent();
    public List<MainAction> Actions { get; set; } = [];
    public MainAction Action { get; set; } = new();
    public List<ExeceteActionUI> ExeceteActions { get; set; } = [];

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await Task.Delay(500);
        var item = (MainActionListViewItem)((ListView)sender).SelectedItem;
        if (item != null)
        {
            string str = item.Title;
            Debug.WriteLine(str);
            Action.Title = str;

            var dataItem = Actions.Find(a => a.Title == str);
            if (dataItem != null)
            {
                subActionsCombox.Items.Clear();
                itemsControl.Items.Clear();
                foreach (var sub in dataItem.SubActions)
                {
                    subActionsCombox.Items.Add(sub.Title);
                    foreach (var exeA in sub.ExeceteActions)
                        itemsControl.Items.Add(new ExeceteActionUI() { Command = exeA.Command });
                }
            }
        }
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        listView.Items.Add(new MainActionListViewItem { Title = "新操作项" });
        ExeceteActions.Add(new() { Command = "powershell" });
        itemsControl.Items.Add(new ExeceteActionUI() { Command = "powershell" });
    }

    private async void LastControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(900);
        for(int i = 1; i <= 5; i++)
        {
            Action = new() { Title = $"操作项 {i}" };

            SubAction sub = new() { Title = $"{i} 子项 1" };
            sub.ExeceteActions.Add(new() { Command = "1 1" });
            sub.ExeceteActions.Add(new() { Command = "1 2" });

            SubAction sub1 = new() { Title = $"{i} 子项 2" };
            sub1.ExeceteActions.Add(new() { Command = "2 1" });
            sub1.ExeceteActions.Add(new() { Command = "2 2" });

            Action.SubActions.Add(sub);
            Action.SubActions.Add(sub1);
            Actions.Add(Action);
        }

        foreach(var item in Actions)
            listView.Items.Add(new MainActionListViewItem { Title = item.Title });
    }
}