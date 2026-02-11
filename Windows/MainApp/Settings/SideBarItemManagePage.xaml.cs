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
    public SubAction SubAction { get; set; } = new();
    public List<ExeceteActionUI> ExeceteActions { get; set; } = [];
    readonly ActionDataManager DataManager = new();


    private int LastActionSelectIndex = -1;
    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (MainActionUI)listView.SelectedItem;
        int index = listView.SelectedIndex;
        if (item != null)
        {
            var dataItem = Actions[index];
            subActionsCombox.Items.Clear();
            itemsControl.Items.Clear();
            SubAction.Title = "";
            foreach (var sub in dataItem.SubActions)
            {
                subActionsCombox.Items.Add(new SubActionUI() { Title = sub.Title, ID = sub.ID });
            }
            Action.ID = dataItem.ID;
            Action.Title = dataItem.Title;
            Action.SubActions = dataItem.SubActions;
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
            var sub = Action.SubActions[SubActionsComboxSelectIndex];
            itemsControl.Items.Clear();
            SubAction.ID = sub.ID;
            SubAction.ExeceteActions = sub.ExeceteActions;
            SubAction.Title = sub.Title;
            foreach (var exeAc in SubAction.ExeceteActions)
                itemsControl.Items.Add(exeAc);
            LastSubActionsComboxSelectIndex = SubActionsComboxSelectIndex;
        }
        else
            SubAction.Title = "";
    }

    private async void LastControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(300);
        Actions = await DataManager.LoadActionsAsync();

        foreach (var item in Actions)
            listView.Items.Add(new MainActionUI { Title = item.Title, ID = item.ID });
    }

    private void DelOperation_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        Debug.WriteLine(Tag);
        foreach (var item in Action.SubActions[subActionsCombox.SelectedIndex].ExeceteActions)
        {
            if(item.ID == Tag)
            {
                Action.SubActions[SubActionsComboxSelectIndex].ExeceteActions.Remove(item);
                itemsControl.Items.Remove(item);
                break;
            }
        }
        _ = DataManager.SaveActionsAsync(Actions);
    }

    private void AddOpreation_HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (SubActionsComboxSelectIndex != -1)
        {
            ExeceteActionUI execete = new();
            Action.SubActions[SubActionsComboxSelectIndex].ExeceteActions.Add(execete);
            itemsControl.Items.Add(execete);
            _ = DataManager.SaveActionsAsync(Actions);
        }
    }

    private void AddFuncution_HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SubAction sub = new() { Title = "新子功能" };
        Action.SubActions.Add(sub);
        subActionsCombox.Items.Add(new SubActionUI() { Title = sub.Title, ID = sub.ID });
        subActionsCombox.SelectedIndex = Action.SubActions.Count - 1;
        _ = DataManager.SaveActionsAsync(Actions);
    }

    private void DelFuncution_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (SubActionsComboxSelectIndex != -1)
        {
            Action.SubActions.RemoveAt(SubActionsComboxSelectIndex);
            SubActionsComboxSelectIndex = -1;
            subActionsCombox.Items.Clear();
            itemsControl.Items.Clear();
            foreach (var sub in Action.SubActions)
            {
                subActionsCombox.Items.Add(new SubActionUI() { Title = sub.Title , ID = sub.ID});
            }
            SubAction.Title = "";
        }
        _ = DataManager.SaveActionsAsync(Actions);
    }

    private async void SubActionTitle_TextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (SubActionsComboxSelectIndex != -1)
        {
            await Task.Delay(20);
            Action.SubActions[SubActionsComboxSelectIndex].Title = SubAction.Title;
            subActionsCombox.Items.Clear();
            foreach (var sub in Action.SubActions)
            {
                subActionsCombox.Items.Add(new SubActionUI() { Title = sub.Title, ID = sub.ID });
            }
            subActionsCombox.SelectedIndex = LastSubActionsComboxSelectIndex;
        }
        _ = DataManager.SaveActionsAsync(Actions);
    }

    private async void ActionTitle_TextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(20);
        int index0 = LastActionSelectIndex;
        int index1 = SubActionsComboxSelectIndex;
        Actions[index0].Title = Action.Title;
        listView.Items.Clear();
        foreach (var item in Actions)
            listView.Items.Add(new MainActionUI { Title = item.Title, ID = item.ID });
        listView.SelectedIndex = index0;
        await Task.Delay(100);
        subActionsCombox.SelectedIndex = index1;
        _ = DataManager.SaveActionsAsync(Actions);
    }

    private void NewOperation_HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        MainAction action = new() { Title = "新操作项" };
        Actions.Add(action);
        listView.Items.Add(new MainActionUI { Title = action.Title, ID = action.ID });
        _ = DataManager.SaveActionsAsync(Actions);
    }

    private void DelMainItem_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Actions.RemoveAt(Actions.Count - 1);
        _ = DataManager.SaveActionsAsync(Actions);
        listView.Items.Clear();
        foreach (var item in Actions)
            listView.Items.Add(new MainActionUI { Title = item.Title, ID = item.ID });
    }
}