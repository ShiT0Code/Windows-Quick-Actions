using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MainApp.Settings;
public sealed partial class SideBarItemManagePage : Page
{
    public SideBarItemManagePage() => InitializeComponent();

    public List<FixItem> FixItems { get; set; } = [];
    public FixItem SelectFixItem { get; set; } = new();
    public SubFixItem SelectSubFixItem { get; set; } = new();
    public List<ExecutionItem> ExecutionItems { get; set; } = [];
    private readonly FixItemDataReaderW DataReaderW = new();


    private int LastActionSelectIndex = -1;
    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (FixItemUI)listView.SelectedItem;
        int index = listView.SelectedIndex;
        if (item != null)
        {
            var dataItem = FixItems[index];
            subActionsCombox.Items.Clear();
            itemsControl.Items.Clear();
            foreach (var sub in dataItem.SubItems)
                subActionsCombox.Items.Add(new SubFixItemUI() { Name = sub.Name, ID = sub.ID });
            SelectFixItem.ID = dataItem.ID;
            SelectFixItem.Name = dataItem.Name;
            SelectFixItem.SubItems = dataItem.SubItems;
            LastActionSelectIndex = index;
            dataView.IsEnabled = true;
        }
        else
        {
            dataView.IsEnabled = false;
            LastActionSelectIndex = -1;
        }
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //listView.Items.Add(new MainActionListViewItem { Title = "新操作项" });
        //ExeceteActions.Add(new() { Command = "powershell" });
        //itemsControl.Items.Add(new ExeceteActionUI() { Command = "powershell" });
        //Action.SubActions[SubActionsComboxSelectIndex].ExeceteActions.Add(new());
    }

    int LastSubActionsComboxSelectIndex = -1;
    private int SubActionsComboxSelectIndex = -1;
    private async void SubActionsCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SubActionsComboxSelectIndex = subActionsCombox.SelectedIndex;
        if (SubActionsComboxSelectIndex != -1)
        {
            var sub = SelectFixItem.SubItems[SubActionsComboxSelectIndex];
            itemsControl.Items.Clear();
            SelectSubFixItem.ID = sub.ID;
            SelectSubFixItem.Actions = sub.Actions;
            SelectSubFixItem.Name = sub.Name;
            foreach (var exeAc in SelectSubFixItem.Actions)
            {
                ExecutionItems.Add(exeAc);
                itemsControl.Items.Add(exeAc);
            }
            LastSubActionsComboxSelectIndex = SubActionsComboxSelectIndex;
        }
        else
            SelectSubFixItem.Name = "";
    }

    private async void LastControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(400);
        FixItems = await DataReaderW.LoadActionsAsync();
        foreach (var item in FixItems)
        {
            listView.Items.Add(new FixItemUI
            {
                Name = item.Name,
                ID = item.ID
            });
        }
        await Task.Delay(500);
        progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        progressBar.IsIndeterminate = false;
    }

    private void DelOperation_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        foreach (var item in SelectFixItem.SubItems[subActionsCombox.SelectedIndex].Actions)
        {
            if(item.ID == Tag)
            {
                SelectFixItem.SubItems[SubActionsComboxSelectIndex].Actions.Remove(item);
                itemsControl.Items.Remove(item);
                ExecutionItems.Remove(item);
                break;
            }
        }
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }

    private void AddOpreation_HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (SubActionsComboxSelectIndex != -1)
        {
            ExecutionItem execete = new();
            SelectFixItem.SubItems[SubActionsComboxSelectIndex].Actions.Add(execete);
            itemsControl.Items.Add(execete);
            _ = DataReaderW.SaveActionsAsync(FixItems);
        }
    }

    private void AddFuncution_HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SubFixItem sub = new() { Name = "新子功能" };
        SelectFixItem.SubItems.Add(sub);
        subActionsCombox.Items.Add(new SubFixItemUI() { Name = sub.Name, ID = sub.ID });
        subActionsCombox.SelectedIndex = SelectFixItem.SubItems.Count - 1;
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }

    private void DelFuncution_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (SubActionsComboxSelectIndex != -1)
        {
            SelectFixItem.SubItems.RemoveAt(SubActionsComboxSelectIndex);
            SubActionsComboxSelectIndex = -1;
            subActionsCombox.Items.Clear();
            itemsControl.Items.Clear();
            foreach (var sub in SelectFixItem.SubItems)
            {
                subActionsCombox.Items.Add(new SubFixItemUI() { Name = sub.Name , ID = sub.ID});
            }
            SelectSubFixItem.Name = "";
        }
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }

    private async void SubActionTitle_TextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (SubActionsComboxSelectIndex != -1)
        {
            await Task.Delay(20);
            SelectFixItem.SubItems[SubActionsComboxSelectIndex].Name = SelectSubFixItem.Name;
            subActionsCombox.Items.Clear();
            foreach (var sub in SelectFixItem.SubItems)
            {
                subActionsCombox.Items.Add(new SubFixItemUI() { Name = sub.Name, ID = sub.ID });
            }
            subActionsCombox.SelectedIndex = LastSubActionsComboxSelectIndex;
        }
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }

    private async void ActionTitle_TextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(20);
        int index0 = LastActionSelectIndex;
        int index1 = SubActionsComboxSelectIndex;
        FixItems[index0].Name = SelectFixItem.Name;
        listView.Items.Clear();
        foreach (var item in FixItems)
            listView.Items.Add(new FixItemUI { Name = item.Name, ID = item.ID });
        listView.SelectedIndex = index0;
        await Task.Delay(100);
        subActionsCombox.SelectedIndex = index1;
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }

    private void NewOperation_HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        FixItem action = new() { Name = "新操作项" };
        FixItems.Add(action);
        listView.Items.Add(new FixItemUI { Name = action.Name, ID = action.ID });
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }

    private void DelMainItem_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        FixItems.RemoveAt(FixItems.Count - 1);
        _ = DataReaderW.SaveActionsAsync(FixItems);
        listView.Items.Clear();
        foreach (var item in FixItems)
            listView.Items.Add(new FixItemUI { Name = item.Name, ID = item.ID });
    }

    private async void Action_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(50);
        SelectSubFixItem.Actions = new(ExecutionItems);
        SelectFixItem.SubItems[SubActionsComboxSelectIndex] = SelectSubFixItem;
        _ = DataReaderW.SaveActionsAsync(FixItems);
    }
}